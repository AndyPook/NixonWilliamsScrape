using System.IO;
using System.Threading.Tasks;

namespace NixonWilliamsScraper
{
    public interface IPageGetter
    {
        Task<Stream> Get(string path);
    }


}
