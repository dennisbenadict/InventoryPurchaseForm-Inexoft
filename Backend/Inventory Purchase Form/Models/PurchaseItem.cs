using System.ComponentModel.DataAnnotations;

namespace Inventory_Purchase_Form.Models
{
    public class PurchaseItem
    {
        [Key]
        public int PurchaseItemId { get; set; }

        [Required]
        public int PurchaseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        /// <summary>Mandatory when product has RequiresLotTracking = true.</summary>
        [MaxLength(100)]
        public string? LotNumber { get; set; }

        /// <summary>Mandatory when product has RequiresLotTracking = true. Must be >= InvoiceDate.</summary>
        public DateTime? ExpiryDate { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        /// <summary>Purchase rate per unit.</summary>
        [Required]
        public decimal Rate { get; set; }

        /// <summary>Discount percentage (0-100).</summary>
        public decimal DiscountPercent { get; set; }

        /// <summary>Auto-calculated: (Quantity × Rate) × (DiscountPercent / 100).</summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>Quantity × Rate - DiscountAmount.</summary>
        public decimal LineTotal { get; set; }

        /// <summary>Sum of all applicable taxes for this line.</summary>
        public decimal TaxAmount { get; set; }

        /// <summary>LineTotal + TaxAmount.</summary>
        public decimal NetAmount { get; set; }

        // Navigation properties
        public Purchase Purchase { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public ICollection<PurchaseItemTax> PurchaseItemTaxes { get; set; } = new List<PurchaseItemTax>();
    }
}
