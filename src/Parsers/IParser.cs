using System.IO;
using System.Threading.Tasks;

namespace NixonWilliamsScraper.Parsers
{
    public interface IParser<T>
    {
        Task<T> Parse(Stream stream);
    }
}
