namespace Application.PaymentMethod.Params
{
    public class PaymentMethodParams : PagingParams
    {
        public string NameFilter { get; set; }
        public bool? NameSort { get; set; }
        public double? PriceFilter { get; set; }
        public bool? PriceSort { get; set; }
    }
}