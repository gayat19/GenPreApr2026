namespace BankingAPI.Models.DTOs
{
    public class ChatNotificationRequest
    {
        public string User { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
