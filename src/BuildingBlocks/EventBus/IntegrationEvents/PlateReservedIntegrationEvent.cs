namespace BuildingBlocks.IntegrationEvents
{
    public class PlateReservedIntegrationEvent : IntegrationEvent
    {
        public Guid PlateId { get; set; }
        public string Registration { get; set; } = string.Empty;
        public string ReservedBy { get; set; } = string.Empty;
        public DateTime ReservedAt { get; set; }
        public int Status { get; set; }
    }
}
