using System;
using System.Collections.Generic;

namespace Application.PaymentMethod.Resources
{
    public class PaymentMethodResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool ExternalApi { get; set; }
        public List<DeliveryMethodForPaymentResource> DeliveryMethods { get; set; }
    }

    public class DeliveryMethodForPaymentResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}