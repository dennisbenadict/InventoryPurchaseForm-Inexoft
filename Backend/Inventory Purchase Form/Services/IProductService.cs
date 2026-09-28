using Inventory_Purchase_Form.DTOs;

namespace Inventory_Purchase_Form.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(bool? activeOnly);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductCreateDto dto);
        Task<ProductDto?> UpdateAsync(int id, ProductCreateDto dto);
        Task<(bool success, string message)> DeleteAsync(int id);
    }
}
