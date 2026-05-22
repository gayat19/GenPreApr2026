using BankingAPI.Models.DTOs;

namespace BankingAPI.Interfaces
{
    public interface ITokenService
    {
        public string CreateNewToken(TokenRequest request);
    }
}
