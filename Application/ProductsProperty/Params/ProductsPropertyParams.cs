namespace Application.ProductsProperty.Params
{
    public class ProductsPropertyParams : PagingParams
    {
        public string NameFilter { get; set; }
        public bool? NameAsc { get; set; }
        public bool? NameDesc { get; set; }
    }
}