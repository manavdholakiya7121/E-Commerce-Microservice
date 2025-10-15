﻿using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface,HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {

        public async Task<ProductDTO> GetProduct(int productId)
        {
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        public async Task<AppUserDTO> GetUser(int userId)
        {
            var getUser = await httpClient.GetAsync($"/api/products/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;

            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            var order = await orderInterface.FindByIdAsync(orderId);
            if(order is null || order!.Id <= 0)
                return null!;

            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            var productDto = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            return new OrderDetailsDTO(
                order.Id,
                productDto.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDto.Name,
                order.PurchaseQuantity,
                productDto.price,
                productDto.price * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if(!orders.Any()) return null!;

            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;

        }
    }
}
