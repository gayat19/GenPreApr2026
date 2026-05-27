using BankingAPI.Interfaces;
using BankingAPI.Misc;
using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService authenticationService,ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegisterUserResponse>> RegisterUser(RegisterUserRequest request)
        {
            try
            {
                var result = await _authenticationService.Register(request);
                return Ok(result);
            }
            catch(UnableToCreateEntityException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message); 
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> CustomerLogin(LoginRequest request)
        {
            try
            {
                var result = await _authenticationService.Login(request);
                return Ok(result);
            }
            catch (InvalidCredentialException ex)
            {
                _logger.LogWarning("Failed login attempt for username: {Username}. Reason: {Message}", request.Username, ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
