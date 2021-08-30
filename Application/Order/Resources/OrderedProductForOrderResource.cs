using System;

namespace Application.Order.Resources
{
    public class OrderedProductForOrderResource
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public double ProductGrossPrice { get; set; }
        public double ProductPercentageSale { get; set; }
        public double Amount { get; set; }
    }
}