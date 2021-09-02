using System;

namespace Application.Order.Params
{
    public class OrderParams : PagingParams
    {
        public int? NumberFilter { get; set; }
        public bool? NumberSort { get; set; }
        public Guid? PaymentMethodFilter { get; set; }
        public bool? PaymentMethodSort { get; set; }
        public Guid? DeliveryMethodFilter { get; set; }
        public bool? DeliveryMethodSort { get; set; }
        public string StatusFilter { get; set; }
        public bool? StatusSort { get; set; }
    }
}