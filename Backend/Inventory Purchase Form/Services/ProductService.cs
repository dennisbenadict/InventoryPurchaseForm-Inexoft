using Inventory_Purchase_Form.Data;
using Inventory_Purchase_Form.DTOs;
using Inventory_Purchase_Form.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Purchase_Form.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> GetAllAsync(bool? activeOnly)
        {
            var query = _context.Products
                .Include(p => p.ProductTaxes)
                .ThenInclude(pt => pt.Tax)
                .Include(p => p.PurchaseItems)
                .AsQueryable();

            if (activeOnly.HasValue)
            {
                query = query.Where(p => p.IsActive == activeOnly.Value);
            }

            var products = await query.ToListAsync();

            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Category = p.Category,
                CostPrice = p.CostPrice,
                SellingPrice = p.SellingPrice,
                RequiresLotTracking = p.RequiresLotTracking,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                IsUsedInTransaction = p.PurchaseItems.Any(),
                Taxes = p.ProductTaxes.Select(pt => new TaxDto
                {
                    TaxId = pt.Tax.TaxId,
                    TaxName = pt.Tax.TaxName,
                    Percentage = pt.Tax.Percentage,
                    IsCompound = pt.Tax.IsCompound,
                    IsActive = pt.Tax.IsActive,
                    IsDeleted = pt.Tax.IsDeleted,
                    CreatedAt = pt.Tax.CreatedAt,
                    IsUsedInTransaction = pt.Tax.PurchaseItemTaxes != null && pt.Tax.PurchaseItemTaxes.Any()
                }).ToList()
            }).ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _context.Products
                .Include(prod => prod.ProductTaxes)
                .ThenInclude(pt => pt.Tax)
                .Include(prod => prod.PurchaseItems)
                .FirstOrDefaultAsync(prod => prod.ProductId == id);

            if (p == null) return null;

            return new ProductDto
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                Category = p.Category,
                CostPrice = p.CostPrice,
                SellingPrice = p.SellingPrice,
                RequiresLotTracking = p.RequiresLotTracking,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                IsUsedInTransaction = p.PurchaseItems.Any(),
                Taxes = p.ProductTaxes.Select(pt => new TaxDto
                {
                    TaxId = pt.Tax.TaxId,
                    TaxName = pt.Tax.TaxName,
                    Percentage = pt.Tax.Percentage,
                    IsCompound = pt.Tax.IsCompound,
                    IsActive = pt.Tax.IsActive,
                    IsDeleted = pt.Tax.IsDeleted,
                    CreatedAt = pt.Tax.CreatedAt,
                    IsUsedInTransaction = pt.Tax.PurchaseItemTaxes != null && pt.Tax.PurchaseItemTaxes.Any()
                }).ToList()
            };
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                Category = dto.Category,
                CostPrice = dto.CostPrice,
                SellingPrice = dto.SellingPrice,
                RequiresLotTracking = dto.RequiresLotTracking,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            
            if (dto.TaxIds != null && dto.TaxIds.Any())
            {
                var validTaxes = await _context.Taxes.IgnoreQueryFilters()
                    .Where(t => dto.TaxIds.Contains(t.TaxId))
                    .ToListAsync();
                    
                foreach (var tax in validTaxes)
                {
                    product.ProductTaxes.Add(new ProductTax
                    {
                        TaxId = tax.TaxId
                    });
                }
            }
            
            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.ProductId) ?? throw new Exception("Product created but not found.");
        }

        public async Task<ProductDto?> UpdateAsync(int id, ProductCreateDto dto)
        {
            var product = await _context.Products
                .Include(p => p.ProductTaxes)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return null;

            product.ProductCode = dto.ProductCode;
            product.ProductName = dto.ProductName;
            product.Category = dto.Category;
            product.CostPrice = dto.CostPrice;
            product.SellingPrice = dto.SellingPrice;
            product.RequiresLotTracking = dto.RequiresLotTracking;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            _context.ProductTaxes.RemoveRange(product.ProductTaxes);

            if (dto.TaxIds != null && dto.TaxIds.Any())
            {
                var validTaxes = await _context.Taxes.IgnoreQueryFilters()
                    .Where(t => dto.TaxIds.Contains(t.TaxId))
                    .ToListAsync();
                    
                foreach (var tax in validTaxes)
                {
                    product.ProductTaxes.Add(new ProductTax
                    {
                        TaxId = tax.TaxId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<(bool success, string message)> DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return (false, "Product not found");
            }

            if (product.PurchaseItems.Any())
            {
                return (false, "Product is used in transactions and cannot be deleted");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return (true, "Product deleted successfully");
        }
    }
}
