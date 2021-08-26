using System;
using System.Collections.Generic;

namespace Application.Order.Resources
{
    public class OrderResource
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public OrderDetailsForOrderResource OrderDetails { get; set; }
        public PaymentMethodForOrderResource PaymentMethod { get; set; }
        public DeliveryMethodForOrderResource DeliveryMethod { get; set; }
        public List<OrderedProductForOrderResource> OrderedProducts { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime WarrantyIsInForce { get; set; }
    }
}