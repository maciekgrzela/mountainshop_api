using System;

namespace Application.DeliveryMethod.Resources
{
    public class DeliveryMethodResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool Takeaway { get; set; }
    }
}