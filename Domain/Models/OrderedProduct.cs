using System;

namespace Domain.Models
{
    public class OrderedProduct
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public double Amount { get; set; }
    }
}