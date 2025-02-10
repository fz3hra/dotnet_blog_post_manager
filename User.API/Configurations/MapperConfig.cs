using AutoMapper;
using User.API.Data;
using User.API.ModelDtos;

namespace User.API.Configurations;

/// <summary>
/// Configures mappings between data models and DTOs.
/// </summary>
public class MapperConfig : Profile
{
    /// <summary>
    /// Initializes the mapper configuration.
    /// </summary>
    public MapperConfig()
    {
        /// <summary>
        /// Maps the RegisterDto model to the UserApi model and enables reverse mapping.
        /// </summary>
        CreateMap<RegisterDto, UserApi>().ReverseMap();
    }
}