using System.IO;
using System.Threading.Tasks;

namespace NixonWilliamsScraper.Scrapers
{
    public interface IVantageScraper<T>
    {
        Task<T> Get();
        Task<T> Parse(Stream stream);
    }

    public static class IVantageScraperExtensions
    {
        public static async Task<T> Parse<T>(this IVantageScraper<T> scraper, string path)
        {
            return await scraper.Parse(new FileStream(path, FileMode.Open));
        }
    }
}
