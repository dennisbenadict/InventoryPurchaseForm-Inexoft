using Inventory_Purchase_Form.DTOs;

namespace Inventory_Purchase_Form.Services
{
    public interface ITaxService
    {
        Task<List<TaxDto>> GetAllAsync(bool includeDeleted = false);
        Task<TaxDto?> GetByIdAsync(int id);
        Task<TaxDto> CreateAsync(TaxCreateDto dto);
        Task<TaxDto?> UpdateAsync(int id, TaxCreateDto dto);
        Task<(bool success, string message)> DeleteAsync(int id);
    }
}
