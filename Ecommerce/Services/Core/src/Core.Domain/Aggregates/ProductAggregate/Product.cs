using Ardalis.GuardClauses;
using Core.Domain.Aggregates.CartAggregate;
using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.WishlistAggregate;

namespace Core.Domain.Aggregates.ProductAggregate;

public class Product
{
    // TODO check if I can avoid this constructor and only use private constructor
    // Required for Entity Framework Core
    public Product()
    {
    }

    private Product(string name, string barcode, string? description, decimal price, string? image, DateTimeOffset createDate, ProductStatus status, int categoryId)
    {
        Name = name;
        Barcode = barcode;
        Description = description;
        Price = price;
        Image = image;
        CreateDate = createDate;
        Status = status;
        CategoryId = categoryId;
        Wishlists = new List<Wishlist>();
        Carts = new List<Cart>();
    }

    public int Id { get; init; }

    public string Name { get; private set; } = null!;

    public string Barcode { get; private set; } = null!;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public string? Image { get; private set; }

    public DateTimeOffset CreateDate { get; private set; }

    public ProductStatus Status { get; private set; }

    public int CategoryId { get; private set; }

    public virtual Category Category { get; private set; }

    public virtual ICollection<Wishlist> Wishlists { get; private set; }

    public virtual ICollection<Cart> Carts { get; private set; }

    public static Product Create(string name, string barcode, string? description, decimal price, string? image, int categoryId)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NullOrWhiteSpace(barcode);
        Guard.Against.NegativeOrZero(price);
        Guard.Against.NegativeOrZero(categoryId);

        return new Product(name, barcode, description, price, image, DateTimeOffset.UtcNow, ProductStatus.Enabled, categoryId);
    }

    public void UpdateMetadata(string name, string barcode, string? description, decimal price, string? image, int categoryId)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NullOrWhiteSpace(barcode);
        Guard.Against.NegativeOrZero(price);
        Guard.Against.NegativeOrZero(categoryId);

        Name = name;
        Barcode = barcode;
        Description = description;
        Price = price;
        Image = image;
        CategoryId = categoryId;
    }

    public bool BarcodeHasChanged(string barcode)
    {
        Guard.Against.NullOrWhiteSpace(barcode);

        return Barcode != barcode;
    }

    public bool CategoryHasChanged(int categoryId)
    {
        Guard.Against.NegativeOrZero(categoryId);

        return CategoryId != categoryId;
    }
}
