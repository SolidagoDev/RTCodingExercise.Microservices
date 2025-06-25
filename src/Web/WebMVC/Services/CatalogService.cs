using System.Text.Json;

namespace WebMVC.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly ILogger<ICatalogService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _catalogSerivceUri;


        public CatalogService(ILogger<ICatalogService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;

            _catalogSerivceUri = "http://catalog-api/api/v1/plates/";
        }

        public async Task<ViewModels.Catalog> GetPlatesAsync(int page, int take, string? filter = null, string? orderBy = null)
        {
            var uri = API.Catalog.GetAllPlates(_catalogSerivceUri, page, take);
            if (!string.IsNullOrEmpty(filter) || !string.IsNullOrEmpty(orderBy))
            {
                var sep = uri.Contains("?") ? "&" : "?";
                if (!string.IsNullOrEmpty(filter)) uri += $"{sep}filter={Uri.EscapeDataString(filter)}";
                if (!string.IsNullOrEmpty(orderBy)) uri += $"&orderBy={Uri.EscapeDataString(orderBy)}";
            }
            var responseString = await _httpClient.GetStringAsync(uri);

            var plates = JsonSerializer.Deserialize<ViewModels.Catalog>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return plates;
        }

        public async Task InsertPlateAsync(Plate plate)
        {
            var uri = API.Catalog.InsertPlate(_catalogSerivceUri);
            var plateContent = new StringContent(JsonSerializer.Serialize(plate), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, plateContent);

            response.EnsureSuccessStatusCode();

            return;
        }
    }
}
