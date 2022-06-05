using System;
using System.Runtime.Serialization;
using AutoMapper;
using CodingChallenge.Application.AutoMapper;
using CodingChallenge.Application.NFT.Commands.Mint;
using CodingChallenge.Domain.Entities.NFT;
using Xunit;

namespace CodingChallenge.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddProfile<MappingProfile>());

        _mapper = _configuration.CreateMapper();
    }

    [Theory]
    [InlineData(typeof(MintCommand), typeof(NFTRecordEntity))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);
        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }
}
