using Microsoft.AspNetCore.Routing;

namespace Core.Presentation.Endpoints;

public interface IEndpoint
{
    void MapRoutes(IEndpointRouteBuilder routeBuilder);
}
