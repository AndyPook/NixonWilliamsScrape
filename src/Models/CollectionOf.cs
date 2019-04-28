using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NixonWilliamsScraper.Models
{
    public class CollectionOf
    {
        public static CollectionOf<T> Create<T>(IEnumerable<T> items) => CollectionOf<T>.Create(items);
    }

    public class CollectionOf<T> : IReadOnlyCollection<T>
    {
        public static CollectionOf<T> Create(IEnumerable<T> items) => new CollectionOf<T>(items);

        private readonly List<T> items;

        protected CollectionOf() { }

        protected CollectionOf(IEnumerable<T> items)
        {
            this.items = items.ToList();
        }

        public int Count => items.Count;

        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
