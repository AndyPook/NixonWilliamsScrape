using System;
using System.IO;
using System.Threading.Tasks;

namespace NixonWilliamsScraper
{
    public class FileGetter : IPageGetter
    {
        private readonly string rootPath;

        public FileGetter(string rootPath)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
        }

        public Task<Stream> Get(string path)
        {
            var fullPath = Path.Combine(rootPath, VantageCrawler.GetPath(path));
            return Task.FromResult((Stream)File.OpenRead(fullPath));
        }
    }
}
