using System;
using System.Collections.Generic;
using System.Linq;

namespace NixonWilliamsScraper.Models
{
    public class BankTransactions
    {
        public BankTransactions(IEnumerable<BankTransaction> items)
        {
            Transactions = items.ToList();
        }

        public string BankId { get; set; }
        public DateTime YearStart { get; set; }
        public DateTime YearEnd { get; set; }

        public IReadOnlyCollection<BankTransaction> Transactions { get; }
    }
}
