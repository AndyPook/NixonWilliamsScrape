namespace NixonWilliamsScraper.Models
{
    public class BankTransactionAllocation
    {
        public decimal Allocation { get; set; }
        public decimal VAT { get; set; }
        public string Category { get; set; }
        public string Explanation { get; set; }

        public override string ToString() => $"{Allocation} {VAT} {Category} {Explanation}";
    }
}
