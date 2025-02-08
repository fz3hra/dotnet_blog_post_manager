using AutoMapper;
using Post.API.ModelDtos;

namespace Post.API.Configurations;

public class MapperConfig: Profile
{
    public MapperConfig()
    {
        CreateMap<Data.Post, GetPostDto>().ReverseMap();
        
        CreateMap<Data.Post, UpdatePostDto>().ReverseMap();
        
        CreateMap<CreatePostDto, Data.Post>().ReverseMap();
        CreateMap<Data.Post, PostResponse>();
        CreateMap<CreatePostDto, PostResponse>();

        CreateMap<Data.Post, GetPostDetailDto>().ReverseMap();
    }
}