using System;

namespace NixonWilliamsScraper.Models
{
    public class CompanyYear
    {
        public string Start { get; set; }
        public string End { get; set; }
        public bool Current { get; set; }

        public Uri SetYearEnd { get; set; }

        public override string ToString() => $"{Start} {End} {Current} {SetYearEnd}";
    }
}
