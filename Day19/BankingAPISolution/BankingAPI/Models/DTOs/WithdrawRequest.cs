namespace BankingAPI.Models.DTOs
{
    public class WithdrawRequest
    {
        public string FromAccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}