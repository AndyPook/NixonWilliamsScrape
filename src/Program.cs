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
using NixonWilliamsScraper.Models;
using NixonWilliamsScraper.Scrapers;

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
            (string x, string y) p = ("", "");
            FormattableString str = $"x={p.x} y={p.y}";

            foreach (var a in str.GetArguments())
                Console.WriteLine($"{a.ToString()}");
            Done();

            Console.WriteLine("Hello World!");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));

            //await Login("andy.pook@gmail.com", "p1sw0rd");
            //await GetDashboard();

            foreach (var t in await new BankScraper(client).Parse("..\\..\\..\\NW-banks.html"))
                Console.WriteLine(t);
            //await ParseDashboard(new FileStream("..\\..\\..\\NW-dashboard.html", FileMode.Open));
            //foreach (var t in await ParseTransactions(new FileStream("..\\..\\..\\NW-transactions.html", FileMode.Open)))
            //    Console.WriteLine(t);
            //foreach (var t in await ParseTransactionAllocations(new FileStream("..\\..\\..\\NW-allocation.html", FileMode.Open)))
            //    Console.WriteLine(t);
            Done();
        }

        static void Done()
        {
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
}
