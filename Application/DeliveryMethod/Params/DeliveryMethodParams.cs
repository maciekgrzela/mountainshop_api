namespace Application.DeliveryMethod.Params
{
    public class DeliveryMethodParams : PagingParams
    {
        public double? PriceFilter { get; set; }
        public bool? PriceSort { get; set; }
        public string NameFilter { get; set; }
        public bool? NameSort { get; set; }
    }

}