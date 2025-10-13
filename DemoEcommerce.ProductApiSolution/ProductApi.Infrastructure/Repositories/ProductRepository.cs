using eCommerce.SharedLibrary.Interface;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.Repositories
{
    internal class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                var getProduct = await GetByAsync(_=>_.Name!.Equals(entity.Name));
                if(getProduct != null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, "Product with the same name already exists.");

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if(currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name}, Added to Database Successfully");
                else
                    return new Response(false, "Failed to add the product to the database.");
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);

                return new Response(false, "An error occurred while creating the product.");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try 
            { 
                var product = await FindByIdAsync(entity.Id);
                if(product is null)
                    return new Response(false, $"{entity.Name} not found.");

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} deleted successfully.");
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);

                return new Response(false, "An error occurred while creating the product.");
            }   
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);

                throw new Exception("An error occurred while finding the product by ID.");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var product = await context.Products.AsNoTracking().ToListAsync();
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);

                throw new Exception("An error occurred while finding the product by ID.");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);

                throw new InvalidOperationException("An error occurred while finding the product by ID.");
            }  
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name}, not found");

                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();   
                return new Response(true, $"{entity.Name}, Updated Successfully");

            }
            catch (Exception ex)
            {
                LogException.LogError(ex);

                return new Response(false,"An error occurred while finding the product by ID.");
            }
        }
    }
}
