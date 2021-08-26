using System;
using System.Collections.Generic;

namespace Application.Order.Resources
{
    public class OrderForUserResource
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public OrderDetailsForOrderResource OrderDetails { get; set; }
        public PaymentMethodForOrderResource PaymentMethod { get; set; }
        public DeliveryMethodForOrderResource DeliveryMethod { get; set; }
        public List<OrderedProductForUserOrderResource> OrderedProducts { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime WarrantyIsInForce { get; set; }
    }

    public class OrderedProductForUserOrderResource
    {
        public Guid ProductId { get; set; }
        public double Amount { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double NetPrice { get; set; }
        public double PercentageTax { get; set; }
        public double GrossPrice { get; set; }
        public double? PercentageSale { get; set; }
    }
}