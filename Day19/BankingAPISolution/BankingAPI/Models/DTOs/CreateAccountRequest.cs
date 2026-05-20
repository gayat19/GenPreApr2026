namespace BankingAPI.Models.DTOs
{
    public class CreateAccountRequest
    {
        public float Balance { get; set; }
        public string AccountType { get; set; }

        public int CustomerId { get; set; }

    }
}
