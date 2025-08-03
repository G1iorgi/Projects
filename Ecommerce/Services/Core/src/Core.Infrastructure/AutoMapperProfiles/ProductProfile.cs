using AutoMapper;
using Core.Application.Aggregates.ProductAggregate.DTOs;
using Core.Domain.Aggregates.ProductAggregate;

namespace Core.Infrastructure.AutoMapperProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDTO>();
    }
}
