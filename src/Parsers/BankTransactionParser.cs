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
    public class BankTransactionParser : IParser<BankTransactions>
    {
        public async Task<BankTransactions> Parse(Stream stream)
        {
            var doc = await new HtmlParser().ParseDocumentAsync(stream);

            var transTable = doc.QuerySelector("#transactions");
            var transRows = transTable.QuerySelectorAll("tbody tr");
            var result = new BankTransactions(GetTransactions());

            var details = doc.QuerySelectorAll("h3").FirstOrDefault(h => h.ChildElementCount == 0 && h.TextContent == "Account Details");
            result.BankId = GetBankId(details.NextElementSibling);

            var years = Parser.GetYears(doc);
            result.YearStart = years.YearStart;
            result.YearEnd = years.YearEnd;

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
                        MoneyIn = Parser.GetMoney(tds[2]),
                        MoneyOut = Parser.GetMoney(tds[3]),
                        Balance = Parser.GetMoney(tds[4]),
                        IsAllocated = tds[5].ClassName.Contains("allocated")
                    };
                }
            }

            string GetBankId(IElement element)
            {
                var href = element.GetAttribute("action");
                var uri = new Uri(href);
                return uri.Segments.Last();
            }
        }
    }
}
