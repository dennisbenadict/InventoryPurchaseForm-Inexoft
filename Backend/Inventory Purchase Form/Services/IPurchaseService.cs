using Inventory_Purchase_Form.DTOs;

namespace Inventory_Purchase_Form.Services
{
    public interface IPurchaseService
    {
        Task<List<PurchaseDto>> GetAllAsync(string? statusFilter = null);
        Task<PurchaseDto?> GetByIdAsync(int id);
        Task<PurchaseDto> CreateAsync(PurchaseCreateDto dto);
        Task<PurchaseDto?> UpdateAsync(int id, PurchaseCreateDto dto);
        Task<(bool success, string message)> DeleteAsync(int id);
        Task<(bool success, string message, PurchaseDto? purchase)> PostAsync(int id);
    }
}
