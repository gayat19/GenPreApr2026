namespace BankingAPI.Models
{
    public class Transaction
    {
        public int TransactionReferenceNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? FromAccountNumber { get; set; }
        public string? ToAccountNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public Account? FromAccount { get; set; }
        public Account? ToAccount { get; set; }
    }
}
