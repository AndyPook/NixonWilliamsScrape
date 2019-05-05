using System;

namespace NixonWilliamsScraper.Models
{
    public class Expense
    {
        public string Number { get; set; }
        public string SheetName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public decimal Net { get; set; }
        public decimal VAT { get; set; }
        public decimal Gross { get; set; }
        public string Urn { get; set; }
    }
}
