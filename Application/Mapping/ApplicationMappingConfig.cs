using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.DTOs.Product;
using Domain.Entities;
using Mapster;

namespace Application.Mapping
{
    // Inheritng from IRegister (Mapster) interface
    public class ApplicationMappingConfig : IRegister
    {
        // implement the Register method (from IRegister interface) to configure mappings
        public void Register(TypeAdapterConfig config)
        {
            // we register each entity to DTO mapping here (best as code and solid (open/closed principle))
            ConfigureCategoryMapping(config);
            ConfigureProductMapping(config);
            
        }

        private static void ConfigureCategoryMapping(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryDto>()
                .Map(destination=> destination.Products, source=> source.Products);
            config.NewConfig<Category, CategoryDetailsDto>();

            // in create and update requests, we put DTOs first, then the entity, because we want to map from DTO to entity
            // we ignore CategoryId and Products because they aren't needed (CategoryId is auto-generaed (and not found in DTO request), and Products is a collection that will be handled separately, and hence we skip lazy loading of Products when creating or updating)
            config.NewConfig<CreateCategoryRequest, Category>()
                .Ignore(destination => destination.CategoryId)
                .Ignore(destination => destination.Products);

            config.NewConfig<UpdateCategoryRequest, Category>().Ignore(destination => destination.CategoryId).Ignore(destination => destination.Products);
        }

        private static void ConfigureProductMapping(TypeAdapterConfig config)
        {
            // mapper here and used Map to show where we map CategoryName
            config.NewConfig<Product, ProdcutDto>().Map(destination => destination.CategoryName, source => source.Category == null ? null : source.Category.CategoryName);

            config.NewConfig<CreateProductRequest, Product>().Ignore(destiation => destiation.ProductId).Ignore(destination => destination.Category);

            config.NewConfig<UpdateProductRequest, Product>().Ignore(destination => destination.ProductId).Ignore(destination => destination.Category);
        }
    }
}
