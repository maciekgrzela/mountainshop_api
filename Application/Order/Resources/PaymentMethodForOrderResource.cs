using System;

namespace Application.Order.Resources
{
    public class PaymentMethodForOrderResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
    }
}