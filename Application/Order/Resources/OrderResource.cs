using System;

namespace Application.Order.Resources
{
    public class OrderResource
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public OrderDetailsForOrderResource Details { get; set; }
        public PaymentMethodForOrderResource PaymentMethod { get; set; }
        public DeliveryMethodForOrderResource DeliveryMethod { get; set; }
        public double NetPrice { get; set; }
        public double PercentageTax { get; set; }
        public double GrossPrice { get; set; }
        public int TotalAmount { get; set; }
        public DateTime Created { get; set; }
        public DateTime WarrantyIsInForce { get; set; }
    }
}