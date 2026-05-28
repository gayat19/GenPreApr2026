using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        [HttpPost("send")]
        public async Task<IActionResult> Test([FromBody]ChatNotificationRequest request)
        {
           await _hubContext.Clients.All.SendAsync("ReceiveMessage", request.User, request.Message);
            return Ok(new { Message = "Message sent to all clients." });
        }
    }
}
