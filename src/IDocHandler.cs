using System;
using System.Threading.Tasks;

namespace NixonWilliamsScraper
{
    public interface IDocHandler
    {
        Task Handle<T>(string path, T item);
    }
}
