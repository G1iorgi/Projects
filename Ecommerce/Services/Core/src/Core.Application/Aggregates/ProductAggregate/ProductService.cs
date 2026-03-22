using Ardalis.GuardClauses;
using Core.Application.Aggregates.ProductAggregate.Commands;
using Core.Application.Aggregates.ProductAggregate.DTOs;
using Core.Application.Services;
using Core.Domain;
using Core.Domain.Aggregates.ProductAggregate;
using SharedKernel.Exceptions.Category;
using SharedKernel.Exceptions.Product;

namespace Core.Application.Aggregates.ProductAggregate;

public class ProductService(
    IUnitOfWork unitOfWork,
    IMapperService mapperService)
{
    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int pageSize,
        int pageNumber,
        string? name = null,
        string? barcode = null,
        string? description = null,
        decimal? priceFrom = null,
        decimal? priceTo = null,
        bool? hasImage = null,
        CancellationToken cancellationToken = default)
    {
        var products = unitOfWork.Products.GetAll();

        if (!string.IsNullOrWhiteSpace(name))
        {
            products = products.Where(p => p.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(barcode))
        {
            products = products.Where(p => p.Barcode.Contains(barcode));
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            products = products.Where(p => p.Description != null && p.Description.Contains(description));
        }

        if (priceFrom.HasValue)
        {
            products = products.Where(p => p.Price >= priceFrom.Value);
        }

        if (priceTo.HasValue)
        {
            products = products.Where(p => p.Price <= priceTo.Value);
        }

        products = hasImage switch
        {
            true => products.Where(p => !string.IsNullOrWhiteSpace(p.Image)),
            false => products.Where(p => string.IsNullOrWhiteSpace(p.Image)),
            _ => products
        };

        var pagedList = await unitOfWork.Products.ToPagedList(products.OrderBy(p => p.Id),
            pageSize,
            pageNumber,
            cancellationToken);
        return mapperService.Map<IEnumerable<ProductDTO>>(pagedList);
    }

    public async Task CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        var isUnique = await unitOfWork.Products.IsUniqueAsync(command.Barcode, cancellationToken);
        if (!isUnique)
        {
            throw new ProductBarcodeAlreadyExistsException(command.Barcode);
        }

        var category = await unitOfWork.Categories.GetByIdAsync(command.CategoryId, cancellationToken);

        if (category == null)
        {
            throw new CategoryNotFoundException(command.CategoryId);
        }

        // Create a new product
        var product = Product.Create(command.Name,
            command.Barcode,
            command.Description,
            command.Price,
            command.Image,
            command.Quantity,
            command.CategoryId);

        await unitOfWork.Products.CreateAsync(product, cancellationToken);

        category.AddProduct(product);
        unitOfWork.Categories.Update(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProductDTO> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);

        if (product == null)
            throw new ProductNotFoundException(id);

        return mapperService.Map<ProductDTO>(product);
    }

    public async Task<IEnumerable<ProductDTO>> GetProductsByIdsAsync(GetProductsByIdsCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        var products = await unitOfWork.Products.GetByIdsAsync(command.ProductIds, cancellationToken);

        var missingIds = command.ProductIds.Except(products.Select(p => p.Id)).ToList();
        if (missingIds is { Count: > 0 })
            throw new ProductNotFoundException(missingIds);

        return mapperService.Map<IEnumerable<ProductDTO>>(products);
    }

    public async Task UpdateProductAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        var product = await unitOfWork.Products.GetByIdAsync(command.Id, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(command.Id);
        }

        if (product.BarcodeHasChanged(command.Barcode))
        {
            var isUnique = await unitOfWork.Products.IsUniqueAsync(command.Barcode, cancellationToken);
            if (!isUnique)
            {
                throw new ProductBarcodeAlreadyExistsException(command.Barcode);
            }
        }

        if (product.CategoryHasChanged(command.CategoryId))
        {
            var oldCategory = await unitOfWork.Categories.GetByIdAsync(product.CategoryId, cancellationToken);

            if (oldCategory != null)
            {
                oldCategory.RemoveProduct(product);
                unitOfWork.Categories.Update(oldCategory);
            }

            var newCategory = await unitOfWork.Categories.GetByIdAsync(command.CategoryId, cancellationToken);

            if (newCategory == null)
            {
                throw new CategoryNotFoundException(command.CategoryId);
            }

            newCategory.AddProduct(product);
            unitOfWork.Categories.Update(newCategory);
        }

        product.UpdateMetadata(command.Name,
            command.Barcode,
            command.Description,
            command.Price,
            command.Image,
            command.Quantity,
            command.CategoryId);
        unitOfWork.Products.Update(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            throw new ProductNotFoundException(id);
        }

        var category = await unitOfWork.Categories.GetByIdAsync(product.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new CategoryNotFoundException(product.CategoryId);
        }

        category.RemoveProduct(product);
        unitOfWork.Categories.Update(category);

        await unitOfWork.Products.DeleteAsync(id, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DecreaseProductQuantityAsync(DecreaseProductsQuantityCommand command,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);
        Guard.Against.Null(command.Items);

        var productIds = command.Items.Select(i => i.ProductId).ToList();
        var products = await unitOfWork.Products.GetByIdsAsync(productIds, cancellationToken);

        foreach (var item in command.Items)
        {
            var product = products.Find(p => p.Id == item.ProductId)
                          ?? throw new ProductNotFoundException(item.ProductId);

            if (!product.HasEnoughQuantity(item.Quantity))
                throw new InsufficientProductQuantityException(item.ProductId);

            product.DecreaseQuantity(item.Quantity);
            unitOfWork.Products.Update(product);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task IncreaseProductsQuantityAsync(IncreaseProductsQuantityCommand command,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);
        Guard.Against.Null(command.Items);

        var productIds = command.Items.Select(i => i.ProductId).ToList();
        var products = await unitOfWork.Products.GetByIdsAsync(productIds, cancellationToken);

        foreach (var item in command.Items)
        {
            var product = products.Find(p => p.Id == item.ProductId)
                          ?? throw new ProductNotFoundException(item.ProductId);

            product.IncreaseQuantity(item.Quantity);
            unitOfWork.Products.Update(product);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
