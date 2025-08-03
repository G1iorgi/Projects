using Ardalis.GuardClauses;
using Core.Domain.Aggregates.ProductAggregate;

namespace Core.Domain.Aggregates.CategoryAggregate;

public class Category
{
    // TODO check if I can avoid this constructor and only use private constructor
    // Required for Entity Framework Core
    public Category()
    {
        Products = new List<Product>();
    }

    private Category(string name, DateTimeOffset createDate, CategoryStatus status)
    {
        Name = name;
        CreateDate = createDate;
        Status = status;
        Products = new List<Product>();
    }

    public int Id { get; init; }

    public string Name { get; private set; } = null!;

    public DateTimeOffset CreateDate { get; private set; }

    public int ProductQuantity => Products.Count;

    public CategoryStatus Status { get; private set; }

    public virtual ICollection<Product> Products { get; private set; }

    public static Category Create(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);

        return new Category(name, DateTimeOffset.UtcNow, CategoryStatus.Enabled);
    }

    public void UpdateMetadata(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);

        Name = name;
    }

    public void AddProduct(Product product)
    {
        Guard.Against.Null(product);

        Products.Add(product);
    }

    public bool RemoveProduct(Product product)
    {
        Guard.Against.Null(product);

        return Products.Remove(product);
    }

    public bool NameHasChanged(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);

        return Name != name;
    }
}
