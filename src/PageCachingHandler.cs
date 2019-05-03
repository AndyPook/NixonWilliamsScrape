using System;
using System.IO;
using System.Threading.Tasks;

namespace NixonWilliamsScraper
{
    public class PageCachingHandler : IPageHandler
    {
        private readonly IPageHandler inner;
        private readonly string rootPath;

        public PageCachingHandler(IPageHandler inner, string rootPath)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));

            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException(nameof(rootPath));
            this.rootPath = rootPath;
        }

        public async Task<T> Handle<T>(string path, Stream stream)
        {
            var memory = new MemoryStream();
            stream.CopyTo(memory);
            memory.Position = 0;
            var fullPath = Path.Combine(rootPath, VantageCrawler.GetPath(path));

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (var file = File.Create(fullPath))
                await memory.CopyToAsync(file);
            memory.Position = 0;

            return await inner.Handle<T>(path, memory);
        }
    }
}
