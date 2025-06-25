namespace Catalog.API.ViewModels
{
    public class DiscountRequestDto
    {
        public Guid PlateId { get; set; }
        public string PromoCode { get; set; } = string.Empty;
    }
}
