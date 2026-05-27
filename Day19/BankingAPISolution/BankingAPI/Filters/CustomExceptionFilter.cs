using BankingAPI.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Authentication;

namespace BankingAPI.Filters
{
    public class CustomExceptionFilter :ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if(context.Exception is UnableToCreateEntityException)
            {
                context.Result = new BadRequestObjectResult(context.Exception.Message);
            }
            else if (context.Exception is InvalidCredentialException)
            {
                context.Result = new UnauthorizedObjectResult(context.Exception.Message);
            }
            else
            {
                context.Result = new ObjectResult(context.Exception.Message)
                {
                    StatusCode = 500
                };
            }
        }
    }
}
