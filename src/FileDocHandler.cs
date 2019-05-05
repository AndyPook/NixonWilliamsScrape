using System;
using Newtonsoft.Json;
using NixonWilliamsScraper.Models;
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
            var fullPath = Path.Combine(rootPath, GetPath(item));
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

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

        private string GetPath<T>(T item)
        {
            switch (item)
            {
                case Banks _: return "banks.json";
                case CompanyYears _: return "years.json";
                case Dashboard d: return $"{d.YearStart.Year}-dashboard.json";
                case BankTransactions t: return $"{t.YearStart.Year}-banktx-{t.BankId}.json";
                case Expenses e: return $"{e.YearStart.Year}-expenses.json";
                case Dividends d: return $"{d.YearStart.Year}-dividends.json";
            }

            throw new ArgumentOutOfRangeException($"doc type unknown: {typeof(T).Name}");
        }
    }
}
