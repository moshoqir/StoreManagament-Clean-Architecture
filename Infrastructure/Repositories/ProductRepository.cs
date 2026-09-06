using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ProductRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<int> CreateAsync(Product product)
        {
            // connection
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            try
            {
                return await connection.QuerySingleAsync<int>("[dbo].[Products_Create]"
                    , new
                    {
                        product.ProductName,
                        product.ProductPrice,
                        product.CategoryId
                    }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number is 2601 or 2627)
            {
                throw new ConflictException("Product name already exists.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            await using var connection = _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            try
            {
                return await connection.QuerySingleAsync<bool>("[dbo].[Products_Delete]"
                    , new { ProductId = productId }
                    , commandType: System.Data.CommandType.StoredProcedure);
            } 
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new ConflictException("Product is in use and can't be deleted.", ex);
            }
        }

        public async Task<bool> ExistsAsync(int productId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            return await connection.QuerySingleAsync<bool>("[dbo].[Products_Exists]"
                , new { ProductId = productId }
                , commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            // here, because we're returning products with their categories, we need to use something called multi-mapping <Product, Category, Product> to map in two objects that are joined.
            var products = await connection.QueryAsync<Product, Category, Product>("[dbo].[Products_GetAll]",
                // since we use this multi-mapping, it's better to make fun and call it where we want.



                // (product, category) =>
                //{
                //    product.Category = category;
                //    return product;
                //},

                MapBetweenProductsAndCategory,

                // we use splitOn to tell the dapper where to start mapping the second object. In this query (see sql), we're splitting on CategoryId (ALWAYS foreign key), so we must tell dapper "start mapping the second object on CategoryId"
                splitOn: "CategoryId",
                commandType: System.Data.CommandType.StoredProcedure);

          return   products.ToList();
        }

        public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            // here, we're returning list of products when we give a categoryId
            var products = await connection.QueryAsync<Product, Category, Product>("[dbo].[Products_GetByCategoryId]",
                MapBetweenProductsAndCategory,
                new { CategoryId = categoryId },
                splitOn: "CategoryId",
                 commandType: System.Data.CommandType.StoredProcedure);

            return products.ToList();
        }

        public async Task<Product?> GetByIdAsync(int productId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var result =  await connection.QueryAsync<Product, Category, Product>("[dbo].[Products_GetById]",
                //(product, category) =>
                //{
                //    product.Category = category;
                //    return product;
                //},

                MapBetweenProductsAndCategory,

                new { ProductId = productId },
                splitOn: "CategoryId"
                
                , commandType: System.Data.CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            try
            {
                return await connection.QuerySingleAsync<bool>("[dbo].[Products_Update]",
                    new
                    {
                        product.ProductId,
                        product.ProductName,
                        product.ProductPrice,
                        product.CategoryId
                    }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number is 2601 or 2627)
            {
                throw new ConflictException("Product name already exists.", ex);
            }
        }

        private static Product MapBetweenProductsAndCategory(Product product, Category category)
        {
            product.CategoryId = category.CategoryId;
            product.Category = category;
            return product;
        }
    }
}
