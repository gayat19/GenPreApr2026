using AutoMapper;

namespace BankingAPI.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.DTOs.RegisterUserRequest, Models.User>();
            CreateMap<Models.DTOs.RegisterUserRequest, Models.Customer>();
            CreateMap<Models.DTOs.CreateAccountRequest, Models.Account>();

        }
    }
}
