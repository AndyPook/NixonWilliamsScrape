using System;
using System.Collections.Generic;
using System.Linq;

namespace NixonWilliamsScraper.Models
{
    public class BankTransactions
    {
        public BankTransactions(IEnumerable<BankTransaction> items)
        {
            Transactions = GetTx().ToList();

            IEnumerable<BankTransaction> GetTx()
            {
                foreach (var tx in items)
                {
                    if (tx.Description == "Opening Balance")
                        Opening = tx;
                    else
                        yield return tx;
                }
            }
        }

        public string BankId { get; set; }
        public DateTime YearStart { get; set; }
        public DateTime YearEnd { get; set; }

        public BankTransaction Opening { get; set; }

        public IReadOnlyCollection<BankTransaction> Transactions { get; }
    }
}
