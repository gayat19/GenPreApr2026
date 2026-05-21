namespace BankingAPI.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public byte[] Password { get; set; } = [];
        public byte[] HashKey { get; set; } = [];
        public string Role { get; set; }= string.Empty;

        public Customer? Customer { get; set; }
    }
}
