using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using System.Linq;
using NixonWilliamsScraper.Models;
using AngleSharp.Dom;
using System;

namespace NixonWilliamsScraper.Parsers
{
    public class CompanyYearsParser : IParser<CompanyYears>
    {
        public async Task<CompanyYears> Parse(Stream stream)
        {
            var doc = await new HtmlParser().ParseDocumentAsync(stream);

            var transTable = doc.QuerySelector("table.auto_links");
            var transRows = transTable.QuerySelectorAll("tbody tr");
            return new CompanyYears(GetYears());

            IEnumerable<CompanyYear> GetYears()
            {
                foreach (var row in transRows)
                {
                    var tds = row.QuerySelectorAll("td");
                    if (!tds.Any())
                        continue;
                    yield return new CompanyYear
                    {
                        Start = tds[0].TextContent,
                        End = tds[1].TextContent,
                        Current = tds[2].TextContent == "Current",
                        SetYearEnd = GetSetYearEnd(tds[3])
                    };
                }
            }

            Uri GetSetYearEnd(IElement element)
            {
                var href = element.Children[0].GetAttribute("href");
                return new Uri(href);
            }
        }
    }
}
