namespace WebMVC.Services
{
    public interface ICatalogService
    {
        Task<ViewModels.Catalog> GetPlatesAsync(int page, int take, string? filter = null, string? orderBy = null);
        Task InsertPlateAsync(Plate plate);
    }
}
