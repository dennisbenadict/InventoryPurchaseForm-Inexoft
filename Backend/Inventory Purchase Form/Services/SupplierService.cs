using Inventory_Purchase_Form.Data;
using Inventory_Purchase_Form.DTOs;
using Inventory_Purchase_Form.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Purchase_Form.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _context;

        public SupplierService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _context.Suppliers.ToListAsync();
            return suppliers.Select(s => new SupplierDto
            {
                SupplierId = s.SupplierId,
                SupplierName = s.SupplierName,
                ContactInfo = s.ContactInfo,
                IsActive = s.IsActive
            }).ToList();
        }

        public async Task<SupplierDto?> GetByIdAsync(int id)
        {
            var s = await _context.Suppliers.FirstOrDefaultAsync(sup => sup.SupplierId == id);
            if (s == null) return null;

            return new SupplierDto
            {
                SupplierId = s.SupplierId,
                SupplierName = s.SupplierName,
                ContactInfo = s.ContactInfo,
                IsActive = s.IsActive
            };
        }

        public async Task<SupplierDto> CreateAsync(SupplierCreateDto dto)
        {
            var supplier = new Supplier
            {
                SupplierName = dto.SupplierName,
                ContactInfo = dto.ContactInfo,
                IsActive = dto.IsActive
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(supplier.SupplierId) ?? throw new Exception("Supplier created but not found.");
        }

        public async Task<SupplierDto?> UpdateAsync(int id, SupplierCreateDto dto)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
            if (supplier == null) return null;

            supplier.SupplierName = dto.SupplierName;
            supplier.ContactInfo = dto.ContactInfo;
            supplier.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }
    }
}
