using System.ComponentModel.DataAnnotations;

namespace Inventory_Purchase_Form.Models
{
    public class ProductTax
    {
        [Key]
        public int ProductTaxId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int TaxId { get; set; }

        // Navigation properties
        public Product Product { get; set; } = null!;
        public Tax Tax { get; set; } = null!;
    }
}
