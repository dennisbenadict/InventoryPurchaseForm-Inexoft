using System.ComponentModel.DataAnnotations;

namespace Inventory_Purchase_Form.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal CostPrice { get; set; }

        [Required]
        public decimal SellingPrice { get; set; }

        public bool RequiresLotTracking { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ProductTax> ProductTaxes { get; set; } = new List<ProductTax>();
        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
