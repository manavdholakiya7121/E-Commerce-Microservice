﻿using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0 ? new Response(true,"Order placed successfully") : new Response(false,"Error Occured while placing order");                  
            }

            catch(Exception ex) 
            {
                LogException.LogError(ex);

                return new Response(false, "Error occured while placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null) return new Response(false, "Error occured while Deleting order");

                context.Orders.Remove(entity);
                return new Response(true, "Order deleted successfully");
            }

            catch (Exception ex)
            {
                LogException.LogError(ex);

                return new Response(false, "Error occured while deleting order");
            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.Orders!.FindAsync(id);
                return order is not null ? order : null!;
            }
            catch (Exception ex) 
            {
                LogException.LogError(ex);
                return null!;
            }           
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await context.Orders.AsNoTracking().ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw new Exception("Error occured while retrieving orders");
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await context.Orders.Where(predicate).FirstOrDefaultAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw new Exception("Error occured while retrieving orders");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await context.Orders.Where(predicate).ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw new Exception("Error occured while placing orders");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var orders = await FindByIdAsync(entity.Id);
                if(orders is null)
                    return new Response(false,"Order not found");

                context.Entry(orders).State = EntityState.Detached;
                context.Orders.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true,"order updated successfully");

            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw new Exception("Error occured while placing orders");
            }
        }
    }
}
