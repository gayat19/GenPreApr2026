namespace BankingAPI.Models.DTOs
{
    public class RegisterUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
