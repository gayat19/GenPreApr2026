using BankingAPI.Interfaces;
using BankingAPI.Models;
using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ICustomerInteract _customerInteract;

        public AccountController(ICustomerInteract customerInteract)
        {
            _customerInteract = customerInteract;
        }
        [HttpPost]
        public async Task<ActionResult<CreateAccountResponse>> CreateAccount(CreateAccountRequest account)
        {
            try
            {
                var result  = await _customerInteract.OpensAccount(account);
                return Created("", result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<GetAccountResponse>> GetAccount(string   accountNumber)
        {
            try
            {
                var account = await _customerInteract.GetAccountByAccountNumber(accountNumber);
                if(account == null) 
                    return NotFound("No account with the given account number - "+accountNumber);
                return Ok(account);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
