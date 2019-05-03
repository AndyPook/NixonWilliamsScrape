using System;
using System.Linq;
using System.Threading.Tasks;
using NixonWilliamsScraper.Models;

namespace NixonWilliamsScraper
{
    public class VantageCrawler
    {
        public const string DashboardPath = "/dashboard/index";
        public const string CompanyYearsPath = "/company/year_end";
        public const string BankAccountsPath = "/bank_accounts";
        public const string BankTransactionsPath = "/bank_accounts/transactions/index/bank_account/";
        public static string GetPath(string path)
        {
            switch (path)
            {
                case DashboardPath: return "dashboard";
                case CompanyYearsPath: return "years";
                case BankAccountsPath: return "banks";
            }

            return path;
        }

        private IPageGetter getter;
        private readonly IPageHandler pageHandler;
        private IDocHandler docHandler;

        private Dashboard dashboard;
        private CompanyYears years;
        private Banks banks;

        public VantageCrawler(IPageGetter getter, IPageHandler pageHandler, IDocHandler docHandler = null)
        {
            this.getter = getter ?? throw new ArgumentNullException(nameof(getter));
            this.docHandler = docHandler;
            this.pageHandler = pageHandler;
        }

        public async Task Crawl()
        {
            Console.WriteLine("--- Dashboard");
            dashboard = await GetDashBoard();
            Console.WriteLine("--- Years");
            years = await GetYears();
            Console.WriteLine("--- Banks");
            banks = await GetBanks();

            //var currentYear = years.FirstOrDefault(y => y.Current);

            foreach (var bank in banks)
            {
                Console.WriteLine($"--- Transactions {bank.BankId}");
                var tx = await GetTransactions(bank.BankId);
            }
        }

        public async Task<Dashboard> GetDashBoard() => await Get<Dashboard>(DashboardPath);

        public async Task<CompanyYears> GetYears() => await Get<CompanyYears>(CompanyYearsPath);

        public async Task<Banks> GetBanks() => await Get<Banks>(BankAccountsPath);

        public async Task<BankTransactions> GetTransactions(string bankId)
            => await Get<BankTransactions>($"/bank_accounts/transactions/index/bank_account/{bankId}");

        public async Task<BankTransactionAllocation> GetTransactionAllocation(string bankId, string dataUrn)
            => await Get<BankTransactionAllocation>($"/bank_accounts/transactions/index/bank_account/{bankId}/urn/{dataUrn}");

        public async Task<T> Get<T>(string path)
        {
            using (var page = await getter.Get(path))
            {
                var doc = await pageHandler.Handle<T>(path, page);
                if (docHandler != null)
                    await docHandler.Handle(path, doc);
                return doc;
            }
        }
    }
}
