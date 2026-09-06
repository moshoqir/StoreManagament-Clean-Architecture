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
    public sealed class CategoryRepository : ICategoryRepository
    {
        private readonly ISqlConnectionFactory ConnectionFactory;

        // it'll handle with db directly.

        // we put ISqlConnectionFactory that already put and deals with dapper
        public CategoryRepository(ISqlConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
           
        }

      

        public async Task<int> CreateAsync(Category category)
        {
            await using var connection = ConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            try
            {
                // we use QuerySingleAsync
                return await connection.QuerySingleAsync<int>("[dbo].[Category_Create]",
                    new { category.CategoryName }, // what is actually will be new
                    commandType: System.Data.CommandType.StoredProcedure);
            }
            // these numbers are coming back from sql. So we can trace them and know them
            catch (SqlException ex) when (ex.Number is 2601 or 2627)
            {
                throw new ConflictException("Category name already exists", ex);
            }
        }

        public async Task<bool> DeleteAsync(int categoryId)
        {
            await using var connection = ConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            try
            {
                return await connection.QuerySingleAsync<bool>("[dbo].[Category_Delete]"
                    , new { CategoryId = categoryId }
                    , commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new ConflictException("Category can't be deleted because it contains products.", ex);
            }
        }

        public async Task<bool> ExistsAsync(int categoryId)
        {
            await using var connection = ConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            // true or false
            return await connection.QuerySingleAsync<bool>("[dbo].[Category_Exists]", new {CategoryId = categoryId}, commandType: System.Data.CommandType.StoredProcedure);


        }

        public async Task<IReadOnlyList<Category>> GetAllAsync()
        {
            //this to create connectio and open it (in dapper, we must control when we open and close connection to db)

            // create connection
            await using var connection = ConnectionFactory.CreateConnection();

            // open connection
            await connection.OpenAsync();

            // there are many types to deal with in dapper QueryAsync is one of them (see the others and understand them)
            var categories = await connection.QueryAsync<Category>
                ("[dbo].[Category_GetAll]", commandType: System.Data.CommandType.StoredProcedure);

            return categories.ToList();
        }

        public async Task<Category?> GetByIdAsync(int categoryId)
        {
            
            await using var conncetion = ConnectionFactory.CreateConnection();

            await conncetion.OpenAsync();


            // QuerySingleOrDefaultAsync is for returning one record
            return await conncetion.QuerySingleOrDefaultAsync<Category>("[dbo].[Category_GetById]", new { CategoryId = categoryId }, commandType: System.Data.CommandType.StoredProcedure);


        }

        public async Task<Category?> GetByIdWithProductsAsync(int categoryId)
        {
           await using var connection = ConnectionFactory.CreateConnection();

           await connection.OpenAsync();

            // QueryMultipleAsync is for multiple records with id

            // here, because the stored procedure returns two tables, we don't call them here, we call one by one then return data
            await using var result = await connection.QueryMultipleAsync("[dbo].[Category_GetByIdWithProducts]", new { CategoryId = categoryId }, commandType: System.Data.CommandType.StoredProcedure);

            // 1. call category record:
            // here, it'll return on record from category, so use SingleOrDefaultAsync
            var category = await result.ReadSingleOrDefaultAsync<Category>();

            // check nullable
            if (category is null)
            {
                return null;
            }

            // 2. call products:
            // here, we return list of products, so readAsync then ToList
            var products = (await result.ReadAsync<Product>()).ToList();

            // 3. for each to call the right data in the Category that's connected to CategoryId:

            foreach (var product in products)
            {
                product.Category = new Category
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName
                };
            }

            // 4. return the data  in the List
            category.Products = products;

            return category;

        }

        public async Task<bool> UpdateAsync(Category category)
        {
            await using var connection = ConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            try
            {
                return await connection.QuerySingleAsync<bool>("[dbo].[Category_Update]"
                    , new
                    {
                        category.CategoryId,
                        category.CategoryName
                    }
                    , commandType: System.Data.CommandType.StoredProcedure);
            }
            catch(SqlException ex) when (ex.Number is 2601 or 2627)
            {
                throw new ConflictException("category name already exists"
                    , ex);
            }
        }
    }
}
