using Inventory_Purchase_Form.Data;
using Inventory_Purchase_Form.DTOs;
using Inventory_Purchase_Form.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Purchase_Form.Services
{
    public class TaxService : ITaxService
    {
        private readonly AppDbContext _context;

        public TaxService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaxDto>> GetAllAsync(bool includeDeleted = false)
        {
            var query = _context.Taxes.Include(t => t.PurchaseItemTaxes).AsQueryable();

            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }

            var taxes = await query.ToListAsync();

            return taxes.Select(t => new TaxDto
            {
                TaxId = t.TaxId,
                TaxName = t.TaxName,
                Percentage = t.Percentage,
                IsCompound = t.IsCompound,
                IsActive = t.IsActive,
                IsDeleted = t.IsDeleted,
                CreatedAt = t.CreatedAt,
                IsUsedInTransaction = t.PurchaseItemTaxes.Any()
            }).ToList();
        }

        public async Task<TaxDto?> GetByIdAsync(int id)
        {
            var t = await _context.Taxes.Include(tax => tax.PurchaseItemTaxes)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(tax => tax.TaxId == id);

            if (t == null) return null;

            return new TaxDto
            {
                TaxId = t.TaxId,
                TaxName = t.TaxName,
                Percentage = t.Percentage,
                IsCompound = t.IsCompound,
                IsActive = t.IsActive,
                IsDeleted = t.IsDeleted,
                CreatedAt = t.CreatedAt,
                IsUsedInTransaction = t.PurchaseItemTaxes.Any()
            };
        }

        public async Task<TaxDto> CreateAsync(TaxCreateDto dto)
        {
            var tax = new Tax
            {
                TaxName = dto.TaxName,
                Percentage = dto.Percentage,
                IsCompound = dto.IsCompound,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Taxes.Add(tax);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(tax.TaxId) ?? throw new Exception("Tax created but not found.");
        }

        public async Task<TaxDto?> UpdateAsync(int id, TaxCreateDto dto)
        {
            var tax = await _context.Taxes.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.TaxId == id);

            if (tax == null) return null;

            tax.TaxName = dto.TaxName;
            tax.Percentage = dto.Percentage;
            tax.IsCompound = dto.IsCompound;
            tax.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<(bool success, string message)> DeleteAsync(int id)
        {
            var tax = await _context.Taxes.Include(t => t.PurchaseItemTaxes)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.TaxId == id);

            if (tax == null)
            {
                return (false, "Tax not found");
            }

            if (tax.IsDeleted)
            {
                return (false, "Tax is already deleted");
            }

            if (tax.PurchaseItemTaxes.Any())
            {
                tax.IsDeleted = true;
                await _context.SaveChangesAsync();
                return (true, "Tax soft-deleted because it is used in transactions");
            }
            else
            {
                _context.Taxes.Remove(tax);
                await _context.SaveChangesAsync();
                return (true, "Tax physically deleted");
            }
        }
    }
}
