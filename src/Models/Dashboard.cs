using System.Collections.Generic;

namespace NixonWilliamsScraper.Models
{
    public class Dashboard
    {
        public ICollection<Section> Sections { get; } = new List<Section>();

        public class Section
        {
            public string Heading { get; set; }
            public ICollection<Item> Items { get; } = new List<Item>();
        }
        public class Item
        {
            public string Name { get; set; }
            public decimal Value { get; set; }
            public string Href { get; set; }
        }
    }
}
