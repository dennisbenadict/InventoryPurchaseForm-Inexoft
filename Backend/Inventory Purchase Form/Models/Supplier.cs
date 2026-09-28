using System.ComponentModel.DataAnnotations;

namespace Inventory_Purchase_Form.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [MaxLength(200)]
        public string SupplierName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ContactInfo { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
