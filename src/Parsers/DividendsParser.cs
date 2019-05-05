using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using AngleSharp.Html.Parser;
using NixonWilliamsScraper.Models;

namespace NixonWilliamsScraper.Parsers
{
    public class DividendsParser : IParser<Dividends>
    {
        public async Task<Dividends> Parse(Stream stream)
        {
            var doc = await new HtmlParser().ParseDocumentAsync(stream);

            var transTable = doc.QuerySelectorAll("table.table_list").First();
            var transRows = transTable.QuerySelectorAll("tbody tr");
            var result = new Dividends(GetItems());

            var years = Parser.GetYears(doc);
            result.YearStart = years.YearStart;
            result.YearEnd = years.YearEnd;

            return result;

            IEnumerable<Dividend> GetItems()
            {
                foreach (var row in transRows.Where(r => r.ClassName != "tablesorter-headerRow"))
                {
                    var tds = row.QuerySelectorAll("td");
                    yield return new Dividend
                    {
                        Date = Parser.GetDate(tds[0]),
                        Amount = Parser.GetMoney(tds[1]),
                    };
                }
            }
        }
    }
}
