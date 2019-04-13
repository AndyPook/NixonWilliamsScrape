using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NixonWilliamsScraper.Scrapers
{
    public abstract class ScraperBase<T> : IVantageScraper<T>
    {
        protected readonly HttpClient client;

        public ScraperBase(HttpClient client)
        {
            this.client = client;
        }

        public virtual async Task<T> Get()
        {
            var response = await client.GetAsync("/bank_accounts");
            var stream = await response.Content.ReadAsStreamAsync();
            return await Parse(stream);
        }

        public abstract Task<T> Parse(Stream stream);

        protected static decimal GetMoney(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0m;

            return decimal.Parse(text.TrimStart('£'));
        }
    }
}
