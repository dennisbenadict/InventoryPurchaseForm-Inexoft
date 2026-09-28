using System.ComponentModel.DataAnnotations;

namespace Inventory_Purchase_Form.Models
{
    /// <summary>
    /// Tracks stock quantity per product + lot combination.
    /// CreatedAt enables FIFO ordering (oldest lots first).
    /// </summary>
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [MaxLength(100)]
        public string LotNumber { get; set; } = string.Empty;

        public DateTime? ExpiryDate { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Product Product { get; set; } = null!;
    }
}
