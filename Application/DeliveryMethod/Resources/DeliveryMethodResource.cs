using System;
using System.Collections.Generic;

namespace Application.DeliveryMethod.Resources
{
    public class DeliveryMethodResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public List<PaymentMethodForDeliveryResource> PaymentMethods { get; set; }
    }

    public class PaymentMethodForDeliveryResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool ExternalApi { get; set; }
    }
}