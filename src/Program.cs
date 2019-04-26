using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using System.Linq;
using AngleSharp.Dom;
using NixonWilliamsScraper.Parsers;

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
            //(string x, string y) p = ("", "");
            //FormattableString str = $"x={p.x} y={p.y}";

            //foreach (var a in str.GetArguments())
            //    Console.WriteLine($"{a.ToString()}");
            //Done();

            Console.WriteLine("Hello World!");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));

            //await Login("andy.pook@gmail.com", "p1sw0rd");
            //await GetDashboard();

            Console.WriteLine("----- Banks");
            foreach (var t in await new BankParser().Parse("..\\..\\..\\..\\NW-banks.html"))
                Console.WriteLine(t);

            Console.WriteLine("\n----- Dashboard");
            var dashboard = await new DashboardParser().Parse("..\\..\\..\\..\\NW-dashboard.html");
            foreach (var section in dashboard.Sections)
            {
                Console.WriteLine(section.Heading);
                foreach (var item in section.Items)
                {
                    Console.WriteLine($"  {item.Name} {item.Value}");
                    if (!string.IsNullOrEmpty(item.Href))
                        Console.WriteLine("    " + item.Href);
                }
            }

            Console.WriteLine("\n----- Transactions");
            foreach (var t in await new TransactionParser().Parse("..\\..\\..\\..\\NW-transactions.html"))
                Console.WriteLine(t);

            Console.WriteLine("\n----- Allocations");
            foreach (var t in await new TransactionAllocationParser().Parse("..\\..\\..\\..\\NW-allocation.html"))
                Console.WriteLine(t);

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

        //static async Task<IEnumerable<BankTransaction>> GetTransactions(int bankId)
        //{
        //    var response = await client.GetAsync($"/bank_accounts/transactions/index/bank_account/{bankId}");
        //    var stream = await response.Content.ReadAsStreamAsync();
        //    return await ParseTransactions(stream);
        //}


        //static async Task<IEnumerable<BankTransactionAllocation>> GetAllocation(int bankId, string urn)
        //{
        //    // bank_accounts/transactions/allocate/bank_account/3368/urn/754623

        //    var response = await client.GetAsync($"/bank_accounts/transactions/allocate/bank_account/{bankId}/urn/{urn}");
        //    var stream = await response.Content.ReadAsStreamAsync();
        //    return await ParseTransactionAllocations(stream);
        //}

    }


}
