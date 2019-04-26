using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using System.Linq;
using AngleSharp.Dom;
using NixonWilliamsScraper.Models;

namespace NixonWilliamsScraper.Parsers
{
    public class BankParser : IParser<IEnumerable<Bank>>
    {
        public async Task<IEnumerable<Bank>> Parse(Stream stream)
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
                        Balance = Parser.GetMoney(tds[4].TextContent)
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
    }
}
