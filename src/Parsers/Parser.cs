using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Linq;

namespace NixonWilliamsScraper.Parsers
{
    public static class Parser
    {
        public static decimal GetMoney(IElement element)
        {
            if (element == null || string.IsNullOrWhiteSpace(element.TextContent))
                return 0m;

            return decimal.Parse(element.TextContent.TrimStart('£'));
        }

        public static DateTime GetDate(IElement element) => DateTime.Parse(element.LastChild.Text());

        public static (DateTime YearStart, DateTime YearEnd) GetYears(IHtmlDocument doc)
        {
            var co = doc.QuerySelectorAll("h4").FirstOrDefault(h => h.ChildElementCount == 0 && h.TextContent == "Company Overview");
            var yearsText = co.NextElementSibling.FirstChild.TextContent;
            var years = yearsText.Split('-');
            return (DateTime.Parse(years[0]), DateTime.Parse(years[1]));
        }
    }
}
