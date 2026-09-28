using Inventory_Purchase_Form.Data;
using Inventory_Purchase_Form.DTOs;
using Inventory_Purchase_Form.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Purchase_Form.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly AppDbContext _db;

        public PurchaseService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<PurchaseDto>> GetAllAsync(string? statusFilter = null)
        {
            var query = _db.Purchases
                .Include(p => p.Supplier)
                .AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
                query = query.Where(p => p.Status == statusFilter);

            var purchases = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return purchases.Select(MapToDto).ToList();
        }

        public async Task<PurchaseDto?> GetByIdAsync(int id)
        {
            var purchase = await _db.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.PurchaseItemTaxes)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null) return null;
            return MapToDto(purchase);
        }

        public async Task<PurchaseDto> CreateAsync(PurchaseCreateDto dto)
        {
            var purchase = new Purchase
            {
                SupplierId = dto.SupplierId,
                InvoiceNumber = dto.InvoiceNumber,
                InvoiceDate = dto.InvoiceDate,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow
            };

            // Add line items
            foreach (var itemDto in dto.Items)
            {
                var item = new PurchaseItem
                {
                    ProductId = itemDto.ProductId,
                    LotNumber = itemDto.LotNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    Quantity = itemDto.Quantity,
                    Rate = itemDto.Rate,
                    DiscountPercent = itemDto.DiscountPercent,
                };

                // Calculate line amounts
                CalculateLineAmounts(item);
                purchase.PurchaseItems.Add(item);
            }

            // Calculate purchase totals
            CalculatePurchaseTotals(purchase);

            _db.Purchases.Add(purchase);
            await _db.SaveChangesAsync();

            return (await GetByIdAsync(purchase.PurchaseId))!;
        }

        public async Task<PurchaseDto?> UpdateAsync(int id, PurchaseCreateDto dto)
        {
            var purchase = await _db.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null) return null;
            if (purchase.Status == "Posted")
                throw new InvalidOperationException("Cannot modify a posted purchase.");

            // Update header
            purchase.SupplierId = dto.SupplierId;
            purchase.InvoiceNumber = dto.InvoiceNumber;
            purchase.InvoiceDate = dto.InvoiceDate;

            // Remove existing items and replace
            _db.PurchaseItems.RemoveRange(purchase.PurchaseItems);

            foreach (var itemDto in dto.Items)
            {
                var item = new PurchaseItem
                {
                    PurchaseId = id,
                    ProductId = itemDto.ProductId,
                    LotNumber = itemDto.LotNumber,
                    ExpiryDate = itemDto.ExpiryDate,
                    Quantity = itemDto.Quantity,
                    Rate = itemDto.Rate,
                    DiscountPercent = itemDto.DiscountPercent,
                };
                CalculateLineAmounts(item);
                purchase.PurchaseItems.Add(item);
            }

            CalculatePurchaseTotals(purchase);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<(bool success, string message)> DeleteAsync(int id)
        {
            var purchase = await _db.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null)
                return (false, "Purchase not found.");

            if (purchase.Status == "Posted")
                return (false, "Cannot delete a posted purchase.");

            _db.Purchases.Remove(purchase);
            await _db.SaveChangesAsync();
            return (true, "Draft purchase deleted successfully.");
        }

        public async Task<(bool success, string message, PurchaseDto? purchase)> PostAsync(int id)
        {
            // Use a transaction to ensure all-or-nothing
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var purchase = await _db.Purchases
                    .Include(p => p.Supplier)
                    .Include(p => p.PurchaseItems)
                        .ThenInclude(pi => pi.Product)
                            .ThenInclude(p => p.ProductTaxes)
                                .ThenInclude(pt => pt.Tax)
                    .FirstOrDefaultAsync(p => p.PurchaseId == id);

                if (purchase == null)
                    return (false, "Purchase not found.", null);

                // ── VALIDATION ──
                var validationError = ValidateForPosting(purchase);
                if (validationError != null)
                    return (false, validationError, null);

                // ── CALCULATE & SNAPSHOT TAXES ──
                foreach (var item in purchase.PurchaseItems)
                {
                    // Recalculate line amounts
                    CalculateLineAmounts(item);

                    // Get applicable taxes for this product
                    var productTaxes = item.Product.ProductTaxes
                        .Select(pt => pt.Tax)
                        .Where(t => !t.IsDeleted)
                        .OrderBy(t => t.IsCompound) // Simple taxes first, then compound
                        .ThenBy(t => t.TaxId)
                        .ToList();

                    decimal runningTaxBase = item.LineTotal;
                    decimal totalTax = 0;

                    // Clear any existing tax snapshots
                    var existingTaxes = await _db.PurchaseItemTaxes
                        .Where(pit => pit.PurchaseItemId == item.PurchaseItemId)
                        .ToListAsync();
                    _db.PurchaseItemTaxes.RemoveRange(existingTaxes);

                    foreach (var tax in productTaxes)
                    {
                        decimal taxAmount;
                        if (tax.IsCompound)
                        {
                            taxAmount = Math.Round(runningTaxBase * (tax.Percentage / 100m), 2);
                        }
                        else
                        {
                            taxAmount = Math.Round(item.LineTotal * (tax.Percentage / 100m), 2);
                        }

                        // Create snapshot
                        var snapshot = new PurchaseItemTax
                        {
                            PurchaseItemId = item.PurchaseItemId,
                            TaxId = tax.TaxId,
                            TaxName = tax.TaxName,
                            TaxPercentage = tax.Percentage,
                            IsCompound = tax.IsCompound,
                            TaxAmount = taxAmount
                        };
                        _db.PurchaseItemTaxes.Add(snapshot);

                        totalTax += taxAmount;
                        runningTaxBase = item.LineTotal + totalTax;
                    }

                    item.TaxAmount = totalTax;
                    item.NetAmount = item.LineTotal + totalTax;
                }

                // Recalculate purchase totals
                CalculatePurchaseTotals(purchase);

                // ── UPDATE INVENTORY ──
                foreach (var item in purchase.PurchaseItems)
                {
                    var lotNumber = item.LotNumber ?? string.Empty;

                    var inventory = await _db.Inventories
                        .FirstOrDefaultAsync(i =>
                            i.ProductId == item.ProductId &&
                            i.LotNumber == lotNumber);

                    if (inventory != null)
                    {
                        inventory.Quantity += item.Quantity;
                        inventory.LastUpdated = DateTime.UtcNow;
                    }
                    else
                    {
                        inventory = new Inventory
                        {
                            ProductId = item.ProductId,
                            LotNumber = lotNumber,
                            ExpiryDate = item.ExpiryDate,
                            Quantity = item.Quantity,
                            CreatedAt = DateTime.UtcNow,
                            LastUpdated = DateTime.UtcNow
                        };
                        _db.Inventories.Add(inventory);
                    }
                }

                // ── MARK AS POSTED ──
                purchase.Status = "Posted";
                purchase.PostedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = await GetByIdAsync(id);
                return (true, "Purchase posted successfully.", result);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Posting failed: {ex.Message}", null);
            }
        }

        // ── PRIVATE HELPERS ──

        private string? ValidateForPosting(Purchase purchase)
        {
            if (purchase.Status != "Draft")
                return "Purchase is already posted.";

            if (purchase.SupplierId <= 0 || purchase.Supplier == null || !purchase.Supplier.IsActive)
                return "Invalid or inactive supplier.";

            if (string.IsNullOrWhiteSpace(purchase.InvoiceNumber))
                return "Invoice number is required.";

            if (purchase.PurchaseItems == null || !purchase.PurchaseItems.Any())
                return "At least one line item is required.";

            // Track product+lot combinations to detect duplicates
            var productLotCombinations = new HashSet<string>();

            foreach (var item in purchase.PurchaseItems)
            {
                var product = item.Product;
                if (product == null)
                    return $"Invalid product on line item.";

                if (!product.IsActive)
                    return $"Product '{product.ProductName}' is inactive.";

                if (item.Quantity <= 0)
                    return $"Quantity must be greater than 0 for product '{product.ProductName}'.";

                if (item.Rate <= 0)
                    return $"Rate must be greater than 0 for product '{product.ProductName}'.";

                if (item.DiscountPercent < 0 || item.DiscountPercent > 100)
                    return $"Discount must be between 0 and 100 for product '{product.ProductName}'.";

                // Lot tracking validation
                if (product.RequiresLotTracking)
                {
                    if (string.IsNullOrWhiteSpace(item.LotNumber))
                        return $"Lot number is required for product '{product.ProductName}'.";

                    if (item.ExpiryDate == null)
                        return $"Expiry date is required for product '{product.ProductName}'.";

                    if (item.ExpiryDate < purchase.InvoiceDate)
                        return $"Expiry date for product '{product.ProductName}' (Lot: {item.LotNumber}) is before the invoice date.";
                }

                // Even for non-lot-tracked products, validate expiry if provided
                if (item.ExpiryDate.HasValue && item.ExpiryDate < purchase.InvoiceDate)
                    return $"Expiry date for product '{product.ProductName}' is before the invoice date.";

                // Check duplicate product+lot combinations
                var key = $"{item.ProductId}_{item.LotNumber ?? ""}";
                if (!productLotCombinations.Add(key))
                {
                    var lotInfo = string.IsNullOrEmpty(item.LotNumber) ? "" : $" (Lot: {item.LotNumber})";
                    return $"Duplicate product '{product.ProductName}'{lotInfo} found. Same product can only appear with different lot numbers.";
                }
            }

            return null; // No errors
        }

        private void CalculateLineAmounts(PurchaseItem item)
        {
            item.DiscountAmount = Math.Round(item.Quantity * item.Rate * (item.DiscountPercent / 100m), 2);
            item.LineTotal = Math.Round(item.Quantity * item.Rate - item.DiscountAmount, 2);
            // TaxAmount and NetAmount are set during posting (with tax snapshots)
            // For draft saves, we set them to 0 or use a rough estimate
        }

        private void CalculatePurchaseTotals(Purchase purchase)
        {
            purchase.TotalAmount = purchase.PurchaseItems.Sum(i => i.LineTotal);
            purchase.TotalTax = purchase.PurchaseItems.Sum(i => i.TaxAmount);
            purchase.NetAmount = purchase.TotalAmount + purchase.TotalTax;
        }

        private PurchaseDto MapToDto(Purchase p)
        {
            return new PurchaseDto
            {
                PurchaseId = p.PurchaseId,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier?.SupplierName ?? "",
                InvoiceNumber = p.InvoiceNumber,
                InvoiceDate = p.InvoiceDate,
                TotalAmount = p.TotalAmount,
                TotalTax = p.TotalTax,
                NetAmount = p.NetAmount,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                PostedAt = p.PostedAt,
                Items = p.PurchaseItems?.Select(i => new PurchaseItemDto
                {
                    PurchaseItemId = i.PurchaseItemId,
                    ProductId = i.ProductId,
                    ProductCode = i.Product?.ProductCode ?? "",
                    ProductName = i.Product?.ProductName ?? "",
                    LotNumber = i.LotNumber,
                    ExpiryDate = i.ExpiryDate,
                    Quantity = i.Quantity,
                    Rate = i.Rate,
                    DiscountPercent = i.DiscountPercent,
                    DiscountAmount = i.DiscountAmount,
                    LineTotal = i.LineTotal,
                    TaxAmount = i.TaxAmount,
                    NetAmount = i.NetAmount,
                    Taxes = i.PurchaseItemTaxes?.Select(t => new PurchaseItemTaxDto
                    {
                        TaxId = t.TaxId,
                        TaxName = t.TaxName,
                        TaxPercentage = t.TaxPercentage,
                        IsCompound = t.IsCompound,
                        TaxAmount = t.TaxAmount
                    }).ToList() ?? new List<PurchaseItemTaxDto>()
                }).ToList() ?? new List<PurchaseItemDto>()
            };
        }
    }
}
