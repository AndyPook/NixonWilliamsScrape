using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using System.Linq;
using NixonWilliamsScraper.Models;

namespace NixonWilliamsScraper.Parsers
{
    public class TransactionAllocationParser : IParser<IEnumerable<BankTransactionAllocation>>
    {
        public async Task<IEnumerable<BankTransactionAllocation>> Parse(Stream stream)
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
                        Allocation = Parser.GetMoney(tds[0].TextContent),
                        VAT = Parser.GetMoney(tds[1].TextContent),
                        Category = tds[2].TextContent,
                        Explanation = tds[3].TextContent
                    };
                }
            }
        }
    }
}
