using System.IO;
using System.Threading.Tasks;

namespace NixonWilliamsScraper
{
    public interface IPageHandler
    {
        Task<T> Handle<T>(string path, Stream stream);
    }
}
