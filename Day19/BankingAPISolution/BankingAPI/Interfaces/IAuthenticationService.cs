using BankingAPI.Models.DTOs;

namespace BankingAPI.Interfaces
{
    public interface IAuthenticationService
    {
        public RegisterUserResponse Register(RegisterUserRequest request);
        public LoginResponse Login(LoginRequest request);
    }
}
