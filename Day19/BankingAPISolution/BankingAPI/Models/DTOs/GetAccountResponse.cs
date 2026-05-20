namespace BankingAPI.Models.DTOs
{
    public class GetAccountResponse
    {
        public string AccountNumber { get; set; } = string.Empty;

        public float Balance { get; set; }
        public string AccountType { get; set; }

        public int CustomerId { get; set; }

        public string Status { get; set; }
    }
}
