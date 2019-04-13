using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using System.Linq;
using AngleSharp.Dom;

namespace NixonWilliamsScraper
{
    class Program
    {
        static Uri baseUri = new Uri("https://www.nixonwilliamsvantage.com");
        static CookieContainer cookies = new CookieContainer();
        static HttpClient client = new HttpClient(new HttpClientHandler { CookieContainer = cookies })
        {
            BaseAddress = baseUri
        };

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));

            //await Login("andy.pook@gmail.com", "p1sw0rd");
            //await GetDashboard();

            foreach (var t in await ParseBanks(new FileStream("..\\..\\..\\NW-banks.html", FileMode.Open)))
                Console.WriteLine(t);
            //await ParseDashboard(new FileStream("..\\..\\..\\NW-dashboard.html", FileMode.Open));
            //foreach (var t in await ParseTransactions(new FileStream("..\\..\\..\\NW-transactions.html", FileMode.Open)))
            //    Console.WriteLine(t);
            //foreach (var t in await ParseTransactionAllocations(new FileStream("..\\..\\..\\NW-allocation.html", FileMode.Open)))
            //    Console.WriteLine(t);

            Console.WriteLine("done...");
            Console.ReadLine();
        }

        static async Task Login(string username, string password)
        {
            var request = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "username", username },
                { "password", password },
                { "btn_submit", "Login" }
            });

            var response = await client.PostAsync("/system/login", request);

            Console.WriteLine($"login: {response.StatusCode}");
        }

        static async Task GetDashboard()
        {
            var response = await client.GetAsync("/dashboard/index");
            Console.WriteLine($"dashboard: {response.StatusCode}");
            var page = new HtmlParser();
            var doc = await page.ParseDocumentAsync(await response.Content.ReadAsStreamAsync());

            var parts = doc.QuerySelectorAll("div.dashboardwidgetinner");

            foreach (var part in parts)
            {
                Console.WriteLine($"part: {part.NodeType}");
            }
        }

        static async Task ParseDashboard(Stream stream)
        {
            var page = new HtmlParser();
            var doc = await page.ParseDocumentAsync(stream);

            var h1 = doc.QuerySelector("h1");
            Console.WriteLine(h1.TextContent);

            var parts = doc.QuerySelectorAll("div.dashboardwidgetinner");

            foreach (var part in parts)
            {
                var h3 = part.QuerySelector("h3");
                Console.WriteLine(h3.TextContent.TrimEnd(' ', '?', '>'));

                var trs = part.QuerySelectorAll("tr");
                foreach (var tr in trs)
                {
                    string name;
                    string href = null;
                    //string value;
                    var col1 = tr.Children[0];
                    if (col1.FirstElementChild is IHtmlAnchorElement a)
                    {
                        name = a.Text;
                        href = a.Href;
                    }
                    else
                    {
                        name = col1.TextContent;
                    }
                    Console.WriteLine($"  {name} = {tr.Children[1].TextContent}");
                    if (href != null)
                        Console.WriteLine("    " + href);
                }
            }
        }

        static async Task<IEnumerable<Bank>> GetBanks()
        {
            var response = await client.GetAsync("/bank_accounts");
            var stream = await response.Content.ReadAsStreamAsync();
            return await ParseBanks(stream);
        }

        static async Task<IEnumerable<Bank>> ParseBanks(Stream stream)
        {
            var doc = await new HtmlParser().ParseDocumentAsync(stream);

            var transTable = doc.QuerySelector("table.auto_links");
            var transRows = transTable.QuerySelectorAll("tbody tr");
            return GetBank().ToList();

            IEnumerable<Bank> GetBank()
            {
                foreach (var row in transRows)
                {
                    var tds = row.QuerySelectorAll("td");
                    if (!tds.Any())
                        continue;
                    yield return new Bank
                    {
                        BankId = GetBankId(tds[5]),
                        AccountName = tds[0].TextContent,
                        BankName = tds[1].TextContent,
                        AccountNumber = tds[2].TextContent,
                        SortCode = tds[3].TextContent,
                        Balance = GetMoney(tds[4].TextContent)
                    };
                }
            }

            string GetBankId(IElement element)
            {
                var href = element.Children[0].GetAttribute("href");
                var bankUi = new Uri(href);
                return bankUi.Segments.Last();
            }
        }


        static async Task<IEnumerable<BankTransaction>> GetTransactions(int bankId)
        {
            var response = await client.GetAsync($"/bank_accounts/transactions/index/bank_account/{bankId}");
            var stream = await response.Content.ReadAsStreamAsync();
            return await ParseTransactions(stream);
        }

        static async Task<IEnumerable<BankTransaction>> ParseTransactions(Stream stream)
        {
            var doc = await new HtmlParser().ParseDocumentAsync(stream);

            var transTable = doc.QuerySelector("#transactions");
            var transRows = transTable.QuerySelectorAll("tbody tr");
            return GetTransactions().ToList();

            IEnumerable<BankTransaction> GetTransactions()
            {
                foreach (var row in transRows.Where(r => r.ClassName != "tablesorter-headerRow"))
                {
                    var tds = row.QuerySelectorAll("td");
                    yield return new BankTransaction
                    {
                        Type = row.GetAttribute("data-type"),
                        Urn = row.GetAttribute("data-urn"),
                        Date = tds[0].ChildNodes.OfType<IText>().FirstOrDefault()?.Text,
                        Description = tds[1].TextContent,
                        MoneyIn = GetMoney(tds[2].TextContent.TrimStart('£')),
                        MoneyOut = GetMoney(tds[3].TextContent.TrimStart('£')),
                        Balance = GetMoney(tds[4].TextContent.TrimStart('£')),
                        IsAllocated = tds[5].ClassName.Contains("allocated")
                    };
                }
            }
        }

        static async Task<IEnumerable<BankTransactionAllocation>> GetAllocation(int bankId, string urn)
        {
            // bank_accounts/transactions/allocate/bank_account/3368/urn/754623

            var response = await client.GetAsync($"/bank_accounts/transactions/allocate/bank_account/{bankId}/urn/{urn}");
            var stream = await response.Content.ReadAsStreamAsync();
            return await ParseTransactionAllocations(stream);
        }


        static async Task<IEnumerable<BankTransactionAllocation>> ParseTransactionAllocations(Stream stream)
        {
            var doc = await new HtmlParser().ParseDocumentAsync(stream);

            var transTable = doc.QuerySelector("#manage_allocation");
            var transRows = transTable.QuerySelectorAll("tbody tr.allocation_line");
            return GetAllocations().ToList();

            IEnumerable<BankTransactionAllocation> GetAllocations()
            {
                foreach (var row in transRows)
                {
                    var tds = row.QuerySelectorAll("td.al");
                    yield return new BankTransactionAllocation
                    {
                        Allocation = GetMoney(tds[0].TextContent),
                        VAT = GetMoney(tds[1].TextContent),
                        Category = tds[2].TextContent,
                        Explanation = tds[3].TextContent
                    };
                }
            }
        }

        static decimal GetMoney(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0m;

            return decimal.Parse(text.TrimStart('£'));
        }

    }

    public class Bank
    {
        public string BankId { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public decimal Balance { get; set; }

        public override string ToString()=>$"{BankId} {AccountName} {BankName} {AccountNumber} {SortCode} {Balance}";
    }

    public class BankTransaction
    {
        public string Type { get; set; }
        public string Urn { get; set; }
        public string DateId { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal MoneyIn { get; set; }
        public decimal MoneyOut { get; set; }
        public decimal Balance { get; set; }
        public bool IsAllocated { get; set; }

        public override string ToString() => $"{Date,10} {Description,20}";
    }

    public class BankTransactionAllocation
    {
        public decimal Allocation { get; set; }
        public decimal VAT { get; set; }
        public string Category { get; set; }
        public string Explanation { get; set; }

        public override string ToString() => $"{Allocation} {VAT} {Category} {Explanation}";
    }
}
