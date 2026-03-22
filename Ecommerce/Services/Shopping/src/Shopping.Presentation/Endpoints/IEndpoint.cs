using Microsoft.AspNetCore.Routing;

namespace Shopping.Presentation.Endpoints;

public interface IEndpoint
{
    void MapRoutes(IEndpointRouteBuilder routeBuilder);
}
