using Catalog.Domain;

namespace Catalog.API.Data
{
    public class PlateAuditLog
    {
        public int Id { get; set; }
        public Guid PlateId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? PerformedBy { get; set; }
        public DateTime PerformedAt { get; set; }
        public PlateStatus Status { get; set; }
    }
}
