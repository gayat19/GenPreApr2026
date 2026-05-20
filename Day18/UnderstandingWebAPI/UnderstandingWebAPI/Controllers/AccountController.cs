using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnderstandingWebAPI.Models;

namespace UnderstandingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        static List<Account> accounts = new List<Account>
        {
            new Account{AccountNumber="0009998787",Balance= 100000,OpeninigDate=new DateTime(2026,1,14),Status="Active"},
            new Account{AccountNumber="0009998789",Balance= 100030,OpeninigDate=new DateTime(2026,2,14),Status = "Active"}
        };
        [HttpGet]
        public ActionResult<IEnumerable<Account>> Get()
        {
            if(accounts.Count == 0)
                return NotFound("No Accounts in the bank yet");
            return Ok(accounts);
        }

        [HttpGet("GetAccountByNumebr")]
        public ActionResult<Account> Get(string accountNumber)
        {
            if (accounts.Count == 0)
                return NotFound("No Accounts in the bank yet");
            var account = accounts.SingleOrDefault(a=>a.AccountNumber == accountNumber);
            if (account == null)
                return NotFound("No accont with the given account number");
            return Ok(account);
        }
        [HttpPost]
        public ActionResult<Account> Post([FromBody] Account account)
        {
            accounts.Add(account);
            return Created("https://localhost:7280/api/Account/GetAccountByNumebr?accountNumber="+account.AccountNumber, account);
        }

    }
    
}
