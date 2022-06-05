using AutoMapper;
using CodingChallenge.Application.AutoMapper;

namespace CodingChallenge.Infrastructure.Mappings;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        typeof(MappingProfile).Assembly.ApplyMappingsFromAssembly(this);
    }
}
