namespace WebMVC
{
    public static class API
    {
        public static class Catalog
        {
            public static string GetAllPlates(string baseUri, int page, int take)
            {
                return $"{baseUri}plates?pageIndex={page}&pageSize={take}";
            }

            public static string InsertPlate(string baseUri)
            {
                return $"{baseUri}insertplate";
            }
        }
    }
}
