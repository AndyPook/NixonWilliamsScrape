namespace NixonWilliamsScraper.Models
{
    public class BankTransaction
    {
        public string Type { get; set; }
        public string Urn { get; set; }
        public string DateId { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal MoneyIn { get; set; }
        public decimal MoneyOut { get; set; }
        public decimal Balance { get; set; }
        public bool IsAllocated { get; set; }

        public override string ToString() => $"{Date,10} {Description,20}";
    }
}
