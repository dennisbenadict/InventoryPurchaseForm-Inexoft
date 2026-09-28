using System.ComponentModel.DataAnnotations;

namespace Inventory_Purchase_Form.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        [MaxLength(100)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime InvoiceDate { get; set; }

        /// <summary>Sum of all line totals (before tax).</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>Sum of all tax amounts across all lines.</summary>
        public decimal TotalTax { get; set; }

        /// <summary>TotalAmount + TotalTax.</summary>
        public decimal NetAmount { get; set; }

        /// <summary>"Draft" or "Posted". Posted purchases are immutable.</summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Set when the purchase is posted. Null while in Draft.</summary>
        public DateTime? PostedAt { get; set; }

        // Navigation properties
        public Supplier Supplier { get; set; } = null!;
        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
