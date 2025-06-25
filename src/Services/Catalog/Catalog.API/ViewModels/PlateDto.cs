using Catalog.Domain;

namespace Catalog.API.ViewModels
{
    public class PlateDto
    {
        public Guid Id { get; set; }
        public string? Registration { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public PlateStatus Status { get; set; }
        public string? Letters { get; set; }
        public int Numbers { get; set; }
        public DateTime? ReservedAt { get; set; }
        public DateTime? SoldAt { get; set; }
        public string? ReservedBy { get; set; }
        public string? SoldTo { get; set; }
    }
}
