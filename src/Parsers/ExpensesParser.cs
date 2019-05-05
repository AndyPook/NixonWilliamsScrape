using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using AngleSharp.Html.Parser;
using NixonWilliamsScraper.Models;
using AngleSharp.Dom;

namespace NixonWilliamsScraper.Parsers
{
    public class ExpensesParser : IParser<Expenses>
    {
        public async Task<Expenses> Parse(Stream stream)
        {
            var doc = await new HtmlParser().ParseDocumentAsync(stream);

            var transTable = doc.QuerySelectorAll("table.table_list").First();
            var transRows = transTable.QuerySelectorAll("tbody tr");
            var result = new Expenses(GetItems());

            var years = Parser.GetYears(doc);
            result.YearStart = years.YearStart;
            result.YearEnd = years.YearEnd;

            return result;

            IEnumerable<Expense> GetItems()
            {
                foreach (var row in transRows.Where(r => r.ClassName != "tablesorter-headerRow"))
                {
                    var tds = row.QuerySelectorAll("td");
                    yield return new Expense
                    {
                        Number = tds[0].TextContent,
                        SheetName = tds[1].TextContent,
                        Date = Parser.GetDate(tds[2]),
                        Status = tds[3].TextContent,
                        Net = Parser.GetMoney(tds[4]),
                        VAT = Parser.GetMoney(tds[5]),
                        Gross = Parser.GetMoney(tds[6]),
                        Urn = GetUrn(tds[7])
                    };
                }
            }

            string GetUrn(IElement element)
            {
                var href = element.Children[0].GetAttribute("href");
                var uri = new Uri(href);
                return uri.Segments.Last();
            }
        }
    }
}
