namespace BankingAPI.Models
{
    public class Transaction
    {
        public int TransactionReferenceNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string FromAccountNumber { get; set; } = string.Empty;
        public string ToAccountNumber { get; set; } =string.Empty;
        public string Status { get; set; } = string.Empty;

        public Account? FromAccount { get; set; }
        public Account? ToAccount { get; set; }
    }
}
