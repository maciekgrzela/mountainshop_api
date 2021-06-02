using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ProductsOrders
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Product Product { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public Order Order { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int OrderedAmount { get; set; }
    }
}