
using System.ComponentModel.DataAnnotations;
namespace ProductApi.Application.DTOs
{
    public record ProductDTO
        (
            int Id,
            [Required] string? Name,
            [Required, Range(1, int.MaxValue)] decimal Price,
            [Required, DataType(DataType.Currency)] int Quantity
        );

}
