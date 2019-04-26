using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NixonWilliamsScraper.Parsers
{
    public static class IParserExtensions
    {
        public static async Task<T> Parse<T>(this IParser<T> parser, string path)
        {
            return await parser.Parse(new FileStream(path, FileMode.Open));
        }

        public static async Task<T> Get<T>(this IParser<T> parser, HttpClient client, string url)
        {
            var response = await client.GetAsync(url);
            var stream = await response.Content.ReadAsStreamAsync();
            return await parser.Parse(stream);
        }
    }
}
