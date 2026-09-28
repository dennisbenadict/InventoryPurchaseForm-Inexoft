using Inventory_Purchase_Form.DTOs;

namespace Inventory_Purchase_Form.Services
{
    public interface IInventoryService
    {
        Task<List<InventoryDto>> GetAllAsync(bool excludeExpired = true);
        Task<List<InventoryDto>> GetByProductIdAsync(int productId, bool excludeExpired = true);
    }
}
