namespace Core.Application.Services;

public interface IMapperService
{
    TDestination Map<TDestination>(object source);
}
