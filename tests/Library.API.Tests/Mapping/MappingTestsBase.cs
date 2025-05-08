using AutoMapper;

using Library.API.Mapping;

namespace Library.API.Tests.Mapping;

public class MappingTestsBase
{
    protected readonly IMapper Mapper;

    public MappingTestsBase()
    {
        var configuration = new MapperConfiguration(cfg => {
            cfg.AddProfile<DtoToModelProfile>();
            cfg.AddProfile<ModelToDtoProfile>();
        });
        Mapper = configuration.CreateMapper();
    }

    [Fact] // private to not run on every test
    private void Should_Have_Valid_Configuration()
    {
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}