using BankingAPI.Models.DTOs;

namespace BankingAPI.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<RegisterUserResponse> Register(RegisterUserRequest request);
        public Task<LoginResponse> Login(LoginRequest request);
    }
}
