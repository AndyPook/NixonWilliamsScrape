using System;
using System.IO;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using NixonWilliamsScraper.Models;

namespace NixonWilliamsScraper.Parsers
{
    public class DashboardParser : IParser<Dashboard>
    {
        public async Task<Dashboard> Parse(Stream stream)
        {
            var page = new HtmlParser();
            var doc = await page.ParseDocumentAsync(stream);

            var h1 = doc.QuerySelector("h1");
            Console.WriteLine(h1.TextContent);

            var parts = doc.QuerySelectorAll("div.dashboardwidgetinner");

            var dashboard = new Dashboard();

            foreach (var part in parts)
            {
                var section = new Dashboard.Section();
                dashboard.Sections.Add(section);

                var h3 = part.QuerySelector("h3");
                section.Heading = h3.TextContent.TrimEnd(' ', '?', '>');

                var trs = part.QuerySelectorAll("tr");
                foreach (var tr in trs)
                {
                    var item = new Dashboard.Item();
                    section.Items.Add(item);

                    var col1 = tr.Children[0];
                    if (col1.FirstElementChild is IHtmlAnchorElement a)
                    {
                        item.Name = a.Text;
                        item.Href = a.Href;
                    }
                    else
                    {
                        item.Name = col1.TextContent;
                    }
                    item.Value = Parser.GetMoney(tr.Children[1].TextContent);
                }
            }

            return dashboard;
        }
    }
}
