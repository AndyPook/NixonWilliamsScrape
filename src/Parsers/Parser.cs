namespace NixonWilliamsScraper.Parsers
{
    public static class Parser
    {
        public static decimal GetMoney(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0m;

            return decimal.Parse(text.TrimStart('£'));
        }
    }
}
