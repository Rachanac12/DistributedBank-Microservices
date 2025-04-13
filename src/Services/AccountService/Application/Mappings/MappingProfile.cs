using AccountService.DTOs;
using AccountService.Models;
using AutoMapper;

namespace AccountService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDto>().ReverseMap();
        }
    }
}
