using System.Collections.Generic;

namespace NixonWilliamsScraper.Models
{
    public class BankTransactions : CollectionOf<BankTransaction>
    {
        public BankTransactions(IEnumerable<BankTransaction> items) : base(items) { }
    }
}
