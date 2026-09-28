using System.ComponentModel.DataAnnotations;

namespace Inventory_Purchase_Form.Models
{
    /// <summary>
    /// Stores a SNAPSHOT of tax values at the time of posting.
    /// This preserves historical tax data even if the Tax master record is later modified.
    /// </summary>
    public class PurchaseItemTax
    {
        [Key]
        public int PurchaseItemTaxId { get; set; }

        [Required]
        public int PurchaseItemId { get; set; }

        [Required]
        public int TaxId { get; set; }

        /// <summary>Snapshot of Tax.TaxName at posting time.</summary>
        [Required]
        [MaxLength(100)]
        public string TaxName { get; set; } = string.Empty;

        /// <summary>Snapshot of Tax.Percentage at posting time.</summary>
        [Required]
        public decimal TaxPercentage { get; set; }

        /// <summary>Snapshot of Tax.IsCompound at posting time.</summary>
        public bool IsCompound { get; set; }

        /// <summary>The computed tax amount for this specific tax on this line.</summary>
        public decimal TaxAmount { get; set; }

        // Navigation properties
        public PurchaseItem PurchaseItem { get; set; } = null!;
        public Tax Tax { get; set; } = null!;
    }
}
