using AutoMapper;
using Post.API.ModelDtos;

namespace Post.API.Configurations;

public class MapperConfig: Profile
{
    public MapperConfig()
    {
        CreateMap<Data.Post, GetPostDto>().ReverseMap();
        
        CreateMap<Data.Post, UpdatePostDto>().ReverseMap();
        CreateMap<UpdatePostDto, Data.Post>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        
        CreateMap<CreatePostDto, Data.Post>().ReverseMap();
        CreateMap<Data.Post, PostResponse>();
        CreateMap<CreatePostDto, PostResponse>();

        CreateMap<Data.Post, GetPostDetailDto>().ReverseMap();
    }
}