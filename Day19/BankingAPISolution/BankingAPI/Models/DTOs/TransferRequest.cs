namespace BankingAPI.Models.DTOs
{
    public class TransferRequest
    {
        public string FromAccountNumber { get; set; } = string.Empty;
        public string ToAccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}