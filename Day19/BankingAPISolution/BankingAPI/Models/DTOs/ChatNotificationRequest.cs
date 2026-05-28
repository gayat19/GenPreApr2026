using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models.DTOs
{
    public class ChatNotificationRequest
    {
        [Required(ErrorMessage ="User cannot be empty")]
        public string User { get; set; } = string.Empty;

        [Required(ErrorMessage ="Message cannot be empty")]
        [MinLength(5, ErrorMessage = "Minimum length of message is 5")]
        public string Message { get; set; } = string.Empty;
    }
}
