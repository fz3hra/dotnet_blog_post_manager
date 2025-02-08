using AutoMapper;
using User.API.Data;
using User.API.ModelDtos;

namespace User.API.Configurations;

public class MapperConfig: Profile
{
    // create mapper for authentication
    public MapperConfig()
    {
        CreateMap<RegisterDto, UserApi>().ReverseMap();
    }
}