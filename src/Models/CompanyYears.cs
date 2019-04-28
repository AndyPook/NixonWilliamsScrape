using System.Collections.Generic;

namespace NixonWilliamsScraper.Models
{
    public class CompanyYears : CollectionOf<CompanyYear>
    {
        public CompanyYears(IEnumerable<CompanyYear> items) : base(items) { }
    }
}
