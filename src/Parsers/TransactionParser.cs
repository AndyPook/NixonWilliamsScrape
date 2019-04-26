using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using System.Linq;
using AngleSharp.Dom;
using NixonWilliamsScraper.Models;

namespace NixonWilliamsScraper.Parsers
{
    public class TransactionParser : IParser<CollectionOf<BankTransaction>>
    {
        public async Task<CollectionOf<BankTransaction>> Parse(Stream stream)
        {
            var doc = await new HtmlParser().ParseDocumentAsync(stream);

            var transTable = doc.QuerySelector("#transactions");
            var transRows = transTable.QuerySelectorAll("tbody tr");
            return CollectionOf.Create(GetTransactions());

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
        }
    }
}
