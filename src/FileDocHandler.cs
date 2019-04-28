using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
namespace NixonWilliamsScraper
{
    public class FileDocHandler : IDocHandler
    {
        private readonly string rootPath;

        public FileDocHandler(string rootPath)
        {
            this.rootPath = rootPath;
        }

        public Task Handle<T>(string path, T item)
        {
            var fullPath = Path.Combine(rootPath, VantageCrawler.GetPath(path)) + ".json";
            using (var file = File.CreateText(fullPath))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });
                serializer.Serialize(file, item);
            }

            return Task.CompletedTask;
        }
    }
}
