using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class OrderedProduct
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        [Required, Range(0, double.MaxValue)]
        public double Amount { get; set; }
    }
}