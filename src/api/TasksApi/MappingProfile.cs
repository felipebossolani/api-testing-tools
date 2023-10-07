using AutoMapper;

namespace TasksApi;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<InsertUserCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

    }
}
