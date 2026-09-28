namespace Inventory_Purchase_Form.DTOs
{
    // ── Request DTO (Create/Update a full purchase with line items) ──
    public class PurchaseCreateDto
    {
        public int SupplierId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public List<PurchaseItemCreateDto> Items { get; set; } = new();
    }

    public class PurchaseItemCreateDto
    {
        public int ProductId { get; set; }
        public string? LotNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal DiscountPercent { get; set; }
    }

    // ── Response DTOs ──
    public class PurchaseDto
    {
        public int PurchaseId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal NetAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PostedAt { get; set; }
        public List<PurchaseItemDto> Items { get; set; } = new();
    }

    public class PurchaseItemDto
    {
        public int PurchaseItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? LotNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public List<PurchaseItemTaxDto> Taxes { get; set; } = new();
    }

    public class PurchaseItemTaxDto
    {
        public int TaxId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public decimal TaxPercentage { get; set; }
        public bool IsCompound { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
