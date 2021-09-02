namespace Application.ProductsProperty.Params
{
    public class ProductsPropertyParams : PagingParams
    {
        public string NameFilter { get; set; }
        public bool? NameSort { get; set; }
    }
}