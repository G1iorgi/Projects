using System.Reflection;

namespace Architecture.Tests;

public class ArchitectureTests
{
    private readonly Assembly domainLayerAssembly = typeof(Core.Domain.DependencyInjection).Assembly;
    private readonly Assembly applicationLayerAssembly = typeof(Core.Application.DependencyInjection).Assembly;
    private readonly Assembly infrastructureLayerAssembly = typeof(Core.Infrastructure.DependencyInjection).Assembly;
    private readonly Assembly presentationLayerAssembly = typeof(Core.Presentation.DependencyInjection).Assembly;
    private readonly Assembly apiLayerAssembly = typeof(Core.API.Assembly).Assembly;

    [Fact]
    public void DomainLayer_ShouldNotReference()
    {
        var domainLayerDependencies = domainLayerAssembly.GetReferencedAssemblies();

        var otherLayers = new List<Assembly>
        {
            applicationLayerAssembly,
            infrastructureLayerAssembly,
            presentationLayerAssembly,
            apiLayerAssembly,
        };

        var otherLayersNames = otherLayers
            .Select(otherLayer => otherLayer.GetName().Name)
            .ToList();
        var domainLayerDependenciesNames = domainLayerDependencies
            .Select(domainLayerDependency => domainLayerDependency.Name).
            ToList();

        Assert.DoesNotContain(otherLayersNames, x => domainLayerDependenciesNames.Contains(x));
    }

    [Fact]
    public void ApplicationLayer_ShouldNotReference()
    {
        var applicationLayerDependencies = applicationLayerAssembly.GetReferencedAssemblies();

        var otherLayers = new List<Assembly>
        {
            infrastructureLayerAssembly,
            presentationLayerAssembly,
            apiLayerAssembly,
        };

        var otherLayersNames = otherLayers
            .Select(otherLayer => otherLayer.GetName().Name)
            .ToList();
        var applicationLayerDependenciesNames = applicationLayerDependencies
            .Select(applicationLayerDependency => applicationLayerDependency.Name)
            .ToList();

        Assert.DoesNotContain(otherLayersNames, x => applicationLayerDependenciesNames.Contains(x));
    }

    [Fact]
    public void ApplicationLayer_ShouldReference()
    {
        var applicationLayerDependencies = applicationLayerAssembly.GetReferencedAssemblies();

        var domainLayerName = domainLayerAssembly.GetName().Name;
        var applicationLayerDependenciesNames = applicationLayerDependencies
            .Select(applicationLayerDependency => applicationLayerDependency.Name)
            .ToList();

        Assert.Contains(domainLayerName, applicationLayerDependenciesNames);
    }

    [Fact]
    public void InfrastructureLayer_ShouldNotReference()
    {
        var infrastructureLayerDependencies = infrastructureLayerAssembly.GetReferencedAssemblies();

        var otherLayers = new List<Assembly>
        {
            presentationLayerAssembly,
            apiLayerAssembly,
        };

        var otherLayersNames = otherLayers
            .Select(otherLayer => otherLayer.GetName().Name)
            .ToList();
        var infrastructureLayerDependenciesNames = infrastructureLayerDependencies
            .Select(infrastructureLayerDependency => infrastructureLayerDependency.Name)
            .ToList();

        Assert.DoesNotContain(otherLayersNames, x => infrastructureLayerDependenciesNames.Contains(x));
    }

    [Fact]
    public void InfrastructureLayer_ShouldReference()
    {
        var infrastructureLayerDependencies = infrastructureLayerAssembly.GetReferencedAssemblies();

        var applicationLayerName = applicationLayerAssembly.GetName().Name;
        var infrastructureLayerDependenciesNames = infrastructureLayerDependencies
            .Select(infrastructureLayerDependency => infrastructureLayerDependency.Name)
            .ToList();

        Assert.Contains(applicationLayerName, infrastructureLayerDependenciesNames);
    }

    [Fact]
    public void PresentationLayer_ShouldNotReference()
    {
        var presentationLayerDependencies = presentationLayerAssembly.GetReferencedAssemblies();

        var otherLayers = new List<Assembly>
        {
            infrastructureLayerAssembly,
            apiLayerAssembly,
        };

        var otherLayersNames = otherLayers
            .Select(otherLayer => otherLayer.GetName().Name)
            .ToList();
        var presentationLayerDependenciesNames = presentationLayerDependencies
            .Select(presentationLayerDependency => presentationLayerDependency.Name)
            .ToList();

        Assert.DoesNotContain(otherLayersNames, x => presentationLayerDependenciesNames.Contains(x));
    }

    [Fact]
    public void PresentationLayer_ShouldReference()
    {
        var presentationLayerDependencies = presentationLayerAssembly.GetReferencedAssemblies();

        var infrastructureLayerName = applicationLayerAssembly.GetName().Name;
        var presentationLayerDependenciesNames = presentationLayerDependencies
            .Select(presentationLayerDependency => presentationLayerDependency.Name)
            .ToList();

        Assert.Contains(infrastructureLayerName, presentationLayerDependenciesNames);
    }

    [Fact]
    public void ApiLayer_ShouldReference()
    {
        var apiLayerDependencies = apiLayerAssembly.GetReferencedAssemblies();

        var presentationLayerName = presentationLayerAssembly.GetName().Name;
        var infrastructureLayerName = infrastructureLayerAssembly.GetName().Name;

        var otherLayersNames = new List<string>
        {
            presentationLayerName!,
            infrastructureLayerName!,
        };

        var apiLayerDependenciesNames = apiLayerDependencies
            .Select(apiLayerDependency => apiLayerDependency.Name)
            .ToList();

        Assert.Contains(otherLayersNames, x => apiLayerDependenciesNames.Contains(x));
    }
}
