using System;

namespace NixonWilliamsScraper.Models
{
    public class CompanyYear
    {
        public DateTime YearStart { get; set; }
        public DateTime YearEnd { get; set; }
        public bool Current { get; set; }

        public Uri SetYearEnd { get; set; }

        public override string ToString() => $"{YearStart} {YearEnd} {Current} {SetYearEnd}";
    }
}
