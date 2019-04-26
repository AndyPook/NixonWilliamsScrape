using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NixonWilliamsScraper.Models;
using NixonWilliamsScraper.Parsers;

namespace NixonWilliamsScraper
{
    public class VantageCrawler
    {
        private IPageGetter getter;
        private IDocHandler handler;

        private Dashboard dashboard;
        private Banks banks;

        public async Task Crawl()
        {
            dashboard = await GetDashBoard();
            banks = await GetBanks();
        }

        public async Task<Dashboard> GetDashBoard() => await Get<Dashboard>("/dashboard/index");

        public async Task<Banks> GetBanks() => await GetAndHandle<Banks>("/bank_accounts");

        public async Task<BankTransactions> GetTransactions(string bankId) 
            => await Get<BankTransactions>($"/bank_accounts/transactions/index/bank_account/{bankId}");

        public async Task<BankTransactionAllocation> GetTransactionAllocation(string bankId, string dataUrn)
            => await Get<BankTransactionAllocation>($"/bank_accounts/transactions/index/bank_account/{bankId}/urn/{dataUrn}");

        public async Task<T> GetAndHandle<T>(string path)
        {
            var doc = await Get<T>(path);
            await handler.Handle(doc);
            return doc;
        }

        public async Task<T> Get<T>(string path)
        {
            var page = await getter.Get(path);
            var parser = GetParser<T>();
            var doc = await parser.Parse(page);
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

            throw new InvalidOperationException("Not parser available for " + typeof(T).Name);
        }
    }


}
