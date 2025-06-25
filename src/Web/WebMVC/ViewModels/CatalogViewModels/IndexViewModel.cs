namespace WebMVC.ViewModels.CatalogViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Plate> Plates { get; set; }

        public PaginationInfo PaginationInfo { get; set; }
    }
}
