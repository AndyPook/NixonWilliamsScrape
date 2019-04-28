using System;
using System.IO;
using System.Threading.Tasks;
using NixonWilliamsScraper.Models;
using NixonWilliamsScraper.Parsers;

namespace NixonWilliamsScraper
{
    public class PageHandler : IPageHandler
    {
        public async Task<T> Handle<T>(string path, Stream stream)
        {
            var parser = GetParser<T>();
            var doc = await parser.Parse(stream);
            return doc;
        }

        private IParser<T> GetParser<T>()
        {
            if (typeof(T) == typeof(Banks))
                return (IParser<T>)new BankParser();
            if (typeof(T) == typeof(BankTransactions))
                return (IParser<T>)new TransactionParser();
            if (typeof(T) == typeof(BankTransactionAllocation))
                return (IParser<T>)new TransactionAllocationParser();
            if (typeof(T) == typeof(Dashboard))
                return (IParser<T>)new DashboardParser();
            if (typeof(T) == typeof(CompanyYears))
                return (IParser<T>)new CompanyYearsParser();

            throw new InvalidOperationException("No parser available for " + typeof(T).Name);
        }
    }
}
