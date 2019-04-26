using System.Threading.Tasks;

namespace NixonWilliamsScraper
{
    public interface IDocHandler
    {
        Task Handle<T>(T item);
    }


}
