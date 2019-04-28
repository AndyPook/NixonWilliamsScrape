using System.Collections.Generic;

namespace NixonWilliamsScraper.Models
{
    public class Banks : CollectionOf<Bank>
    {
        public Banks(IEnumerable<Bank> items) : base(items) { }
    }

}
