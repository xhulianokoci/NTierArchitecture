using AutoMapper;
using Entities.Models;
using Shared.DTO;

namespace NTierTemplate;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Authentication
        CreateMap<CreateUserDTO, ApplicationUser>();

        //User
        CreateMap<ApplicationUser, UserListDTO>().ReverseMap();
    }
}
