using OrderApi.Application.DTOs;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public class OrderService(HttpClient httpClient, ResiliencePipeline<string> resiliencePipeline) : IOrderService
    {

        public async Task<ProductDTO> GetProduct(int productId)
        {

        }

        public Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<IAsyncEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
