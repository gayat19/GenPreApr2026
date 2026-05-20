namespace UnderstandingWebAPI.Models
{
    public class Account
    {
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime OpeninigDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
