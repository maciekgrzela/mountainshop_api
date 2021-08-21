namespace Application.DeliveryMethod.Params
{
    public class DeliveryMethodParams : PagingParams
    {
        public double? PriceFilter { get; set; }
        public bool? PriceAsc { get; set; }
        public bool? PriceDesc { get; set; }
        public bool? TakeawayFilter { get; set; }
        public bool? TakeawayAsc { get; set; }
        public bool? TakeawayDesc { get; set; }
    }

}