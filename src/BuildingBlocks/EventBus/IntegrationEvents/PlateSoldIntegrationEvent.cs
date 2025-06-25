namespace BuildingBlocks.IntegrationEvents
{
    public class PlateSoldIntegrationEvent : IntegrationEvent
    {
        public Guid PlateId { get; set; }
        public string Registration { get; set; } = string.Empty;
        public string SoldTo { get; set; } = string.Empty;
        public DateTime SoldAt { get; set; }
        public int Status { get; set; }
    }
}
