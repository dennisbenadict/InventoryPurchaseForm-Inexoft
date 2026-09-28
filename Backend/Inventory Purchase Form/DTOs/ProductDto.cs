namespace Inventory_Purchase_Form.DTOs
{
    // ── Request DTO (for Create/Update) ──
    public class ProductCreateDto
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public bool RequiresLotTracking { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>List of TaxIds to assign to this product.</summary>
        public List<int> TaxIds { get; set; } = new();
    }

    // ── Response DTO ──
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public bool RequiresLotTracking { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>Taxes currently assigned to this product.</summary>
        public List<TaxDto> Taxes { get; set; } = new();

        /// <summary>Whether this product has been used in any purchase transaction.</summary>
        public bool IsUsedInTransaction { get; set; }
    }
}
