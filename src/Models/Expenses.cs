using System.Collections.Generic;
using System;
using System.Linq;

namespace NixonWilliamsScraper.Models
{
    public class Expenses
    {
        public Expenses(IEnumerable<Expense> items)
        {
            Items = items.ToList();
        }

        public DateTime YearStart { get; set; }
        public DateTime YearEnd { get; set; }

        public IReadOnlyCollection<Expense> Items { get; }
    }
}
