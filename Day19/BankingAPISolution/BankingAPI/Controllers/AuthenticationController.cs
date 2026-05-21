using BankingAPI.Interfaces;
using BankingAPI.Misc;
using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Register")]
        public ActionResult<RegisterUserResponse> RegisterUser(RegisterUserRequest request)
        {
            try
            {
                var result = _authenticationService.Register(request);
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
    }
}
