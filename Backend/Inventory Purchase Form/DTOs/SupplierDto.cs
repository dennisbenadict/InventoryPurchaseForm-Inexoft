namespace Inventory_Purchase_Form.DTOs
{
    // ── Request DTO ──
    public class SupplierCreateDto
    {
        public string SupplierName { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    // ── Response DTO ──
    public class SupplierDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
