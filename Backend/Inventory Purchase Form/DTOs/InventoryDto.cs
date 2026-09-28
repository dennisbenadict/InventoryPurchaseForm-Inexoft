namespace Inventory_Purchase_Form.DTOs
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string LotNumber { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public decimal Quantity { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
