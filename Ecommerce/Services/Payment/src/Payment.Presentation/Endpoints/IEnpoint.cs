using Microsoft.AspNetCore.Routing;

namespace Payment.Presentation.Endpoints;

public interface IEndpoint
{
    void MapRoutes(IEndpointRouteBuilder routeBuilder);
}
