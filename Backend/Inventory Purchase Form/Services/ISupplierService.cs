using Inventory_Purchase_Form.DTOs;

namespace Inventory_Purchase_Form.Services
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllAsync();
        Task<SupplierDto?> GetByIdAsync(int id);
        Task<SupplierDto> CreateAsync(SupplierCreateDto dto);
        Task<SupplierDto?> UpdateAsync(int id, SupplierCreateDto dto);
    }
}
