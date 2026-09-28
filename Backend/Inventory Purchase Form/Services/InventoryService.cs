using Inventory_Purchase_Form.Data;
using Inventory_Purchase_Form.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Purchase_Form.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _db;

        public InventoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<InventoryDto>> GetAllAsync(bool excludeExpired = true)
        {
            var query = _db.Inventories
                .Include(i => i.Product)
                .Where(i => i.Quantity > 0)
                .AsQueryable();

            if (excludeExpired)
            {
                query = query.Where(i =>
                    i.ExpiryDate == null || i.ExpiryDate >= DateTime.UtcNow.Date);
            }

            var items = await query
                .OrderBy(i => i.CreatedAt) // FIFO ordering
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        public async Task<List<InventoryDto>> GetByProductIdAsync(int productId, bool excludeExpired = true)
        {
            var query = _db.Inventories
                .Include(i => i.Product)
                .Where(i => i.ProductId == productId && i.Quantity > 0)
                .AsQueryable();

            if (excludeExpired)
            {
                query = query.Where(i =>
                    i.ExpiryDate == null || i.ExpiryDate >= DateTime.UtcNow.Date);
            }

            var items = await query
                .OrderBy(i => i.CreatedAt) // FIFO ordering
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        private InventoryDto MapToDto(Models.Inventory inv)
        {
            return new InventoryDto
            {
                InventoryId = inv.InventoryId,
                ProductId = inv.ProductId,
                ProductCode = inv.Product?.ProductCode ?? "",
                ProductName = inv.Product?.ProductName ?? "",
                LotNumber = inv.LotNumber,
                ExpiryDate = inv.ExpiryDate,
                Quantity = inv.Quantity,
                IsExpired = inv.ExpiryDate.HasValue && inv.ExpiryDate < DateTime.UtcNow.Date,
                CreatedAt = inv.CreatedAt,
                LastUpdated = inv.LastUpdated
            };
        }
    }
}
