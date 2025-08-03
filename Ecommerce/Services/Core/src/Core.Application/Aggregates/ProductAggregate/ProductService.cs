using Ardalis.GuardClauses;
using Core.Application.Aggregates.ProductAggregate.Commands;
using Core.Application.Aggregates.ProductAggregate.DTOs;
using Core.Application.Services;
using Core.Domain;
using Core.Domain.Aggregates.ProductAggregate;

namespace Core.Application.Aggregates.ProductAggregate;

public class ProductService(
    IUnitOfWork unitOfWork,
    IMapperService mapperService)
{
    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(
        int pageSize,
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

        var pagedList = await unitOfWork.Products.ToPagedList(
            products,
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
            throw new ArgumentException($"Product with barcode {command.Barcode} already exists.");
        }

        var category = await unitOfWork.Categories.GetByIdAsync(command.CategoryId, cancellationToken);

        if (category == null)
        {
            throw new ArgumentException($"Category with ID {command.CategoryId} does not exist.");
        }

        // Create a new product
        var product = Product.Create(
            command.Name,
            command.Barcode,
            command.Description,
            command.Price,
            command.Image,
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
            throw new ArgumentException($"Product with ID {id} does not exist.");

        return mapperService.Map<ProductDTO>(product);
    }

    public async Task UpdateProductAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        var product = await unitOfWork.Products.GetByIdAsync(command.Id, cancellationToken);

        if (product == null)
        {
            throw new ArgumentException($"Product with ID {command.Id} does not exist.");
        }

        if (product.BarcodeHasChanged(command.Barcode))
        {
            var isUnique = await unitOfWork.Products.IsUniqueAsync(command.Barcode, cancellationToken);
            if (!isUnique)
            {
                throw new ArgumentException($"A product with the barcode '{command.Barcode}' already exists.");
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
                throw new ArgumentException($"Category with ID {command.CategoryId} does not exist.");
            }

            newCategory.AddProduct(product);
            unitOfWork.Categories.Update(newCategory);
        }

        product.UpdateMetadata(
            command.Name,
            command.Barcode,
            command.Description,
            command.Price,
            command.Image,
            command.CategoryId);
        unitOfWork.Products.Update(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            throw new ArgumentException($"Product with ID {id} does not exist.");
        }

        var category = await unitOfWork.Categories.GetByIdAsync(product.CategoryId, cancellationToken);

        if (category == null)
        {
            throw new ArgumentException($"Category with ID {product.CategoryId} does not exist.");
        }

        category.RemoveProduct(product);

        await unitOfWork.Products.DeleteAsync(id, cancellationToken);

        unitOfWork.Categories.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
