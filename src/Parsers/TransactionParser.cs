using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using NixonWilliamsScraper.Models;

namespace NixonWilliamsScraper.Parsers
{
    public class TransactionParser : IParser<BankTransactions>
    {
        public async Task<BankTransactions> Parse(Stream stream)
        {
            var doc = await new HtmlParser().ParseDocumentAsync(stream);

            var transTable = doc.QuerySelector("#transactions");
            var transRows = transTable.QuerySelectorAll("tbody tr");
            var result = new BankTransactions(GetTransactions());

            var details = doc.QuerySelectorAll("h3").FirstOrDefault(h => h.ChildElementCount == 0 && h.TextContent == "Account Details");
            result.BankId = GetBankId(details.NextElementSibling);

            var co = doc.QuerySelectorAll("h4").FirstOrDefault(h => h.ChildElementCount == 0 && h.TextContent == "Company Overview");
            var yearsText = co.NextElementSibling.FirstChild.TextContent;
            var years = yearsText.Split('-');
            result.YearStart = DateTime.Parse(years[0]);
            result.YearEnd = DateTime.Parse(years[1]);

            return result;

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
                        MoneyIn = Parser.GetMoney(tds[2].TextContent.TrimStart('£')),
                        MoneyOut = Parser.GetMoney(tds[3].TextContent.TrimStart('£')),
                        Balance = Parser.GetMoney(tds[4].TextContent.TrimStart('£')),
                        IsAllocated = tds[5].ClassName.Contains("allocated")
                    };
                }
            }

            string GetBankId(IElement element)
            {
                var href = element.GetAttribute("action");
                var bankUi = new Uri(href);
                return bankUi.Segments.Last();
            }
        }
    }
}
