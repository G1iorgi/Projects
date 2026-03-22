using AutoMapper;
using Core.Application.Services;
using Core.Infrastructure.AutoMapperProfiles;

namespace Core.Infrastructure.Services;

public class MapperService : IMapperService
{
    private readonly IMapper _mapper;

    public MapperService()
    {
        var configuration = new MapperConfiguration(cfg => { cfg.AddMaps(typeof(ProductProfile)); });

        _mapper = configuration.CreateMapper();
    }

    public TDestination Map<TDestination>(object? source)
        => source == null ? default! : _mapper.Map<TDestination>(source);
}
