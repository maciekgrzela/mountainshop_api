using System;

namespace Application.PaymentMethod.Resources
{
    public class PaymentMethodResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
    }
}