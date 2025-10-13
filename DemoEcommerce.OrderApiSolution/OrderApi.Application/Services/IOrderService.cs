using OrderApi.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services
{
    public interface IOrderService
    {
        Task<IAsyncEnumerable<OrderDTO>> GetOrdersByClientId(int clientId);

        Task<OrderDetailsDTO> GetOrderDetails(int orderId);
    }
}
