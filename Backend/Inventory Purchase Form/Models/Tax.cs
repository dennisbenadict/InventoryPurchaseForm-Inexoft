using System.ComponentModel.DataAnnotations;

namespace Inventory_Purchase_Form.Models
{
    public class Tax
    {
        [Key]
        public int TaxId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TaxName { get; set; } = string.Empty;

        [Required]
        public decimal Percentage { get; set; }

        /// <summary>
        /// If true, this tax is applied on (line total + all preceding tax amounts).
        /// If false, this tax is applied only on the line total.
        /// </summary>
        public bool IsCompound { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Soft-delete flag. Taxes used in transactions are never physically deleted.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ProductTax> ProductTaxes { get; set; } = new List<ProductTax>();
        public ICollection<PurchaseItemTax> PurchaseItemTaxes { get; set; } = new List<PurchaseItemTax>();
    }
}
