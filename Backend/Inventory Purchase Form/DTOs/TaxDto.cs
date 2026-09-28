namespace Inventory_Purchase_Form.DTOs
{
    // ── Request DTO ──
    public class TaxCreateDto
    {
        public string TaxName { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public bool IsCompound { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ── Response DTO ──
    public class TaxDto
    {
        public int TaxId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public bool IsCompound { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>Whether this tax has been used in any posted transaction.</summary>
        public bool IsUsedInTransaction { get; set; }
    }
}
