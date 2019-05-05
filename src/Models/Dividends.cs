using System;
using System.Collections.Generic;
using System.Linq;

namespace NixonWilliamsScraper.Models
{
    public class Dividends
    {
        public Dividends(IEnumerable<Dividend> items)
        {
            Items = items.ToList();
        }

        public DateTime YearStart { get; set; }
        public DateTime YearEnd { get; set; }

        public IReadOnlyCollection<Dividend> Items { get; }
    }
}
