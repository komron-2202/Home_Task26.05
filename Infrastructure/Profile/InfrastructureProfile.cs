using AutoMapper;
using Domain.Dtos.Category;
using Domain.Dtos.Order;
using Domain.Dtos.Product;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.AutoMapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        CreateMap<CreateOrderDto, Order>();
        CreateMap<UpdateOrderDto, Order>();
        CreateMap<GetOrderDto, Order>();
      
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();
        CreateMap<GetCategoryDto, Category>();
        
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        CreateMap<GetProductDto, Product>();
        
        
    }
}