using ProductApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Application.DTOs.Conversions
{
    public static class ProductConversions
    {
        public static Product ToEntity(ProductDTO product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity
        };

        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            if (product is null && products is null)
            {
                return (null, null);
            }
            var productDto = product != null ? new ProductDTO(product.Id, product.Name, product.Price, product.Quantity) : null;
            var productsDto = products?.Select(p => new ProductDTO(p.Id, p.Name, p.Price, p.Quantity)).ToList();
            return (productDto, productsDto);
        }
    }
}
