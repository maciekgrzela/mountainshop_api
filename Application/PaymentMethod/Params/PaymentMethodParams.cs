namespace Application.PaymentMethod.Params
{
    public class PaymentMethodParams : PagingParams
    {
        public string NameFilter { get; set; }
        public bool? NameAsc { get; set; }
        public bool? NameDesc { get; set; }
        public double? PriceFilter { get; set; }
        public bool? PriceAsc { get; set; }
        public bool? PriceDesc { get; set; }
    }
}