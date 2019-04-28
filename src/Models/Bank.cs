namespace NixonWilliamsScraper.Models
{
    public class Bank
    {
        public string BankId { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public decimal Balance { get; set; }

        public override string ToString() => $"{BankId} {AccountName} {BankName} {AccountNumber} {SortCode} {Balance}";
    }
}
