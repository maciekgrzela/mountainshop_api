using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Number { get; set; }
        [Required]
        public Guid OrderDetailsId { get; set; }
        [Required]
        public OrderDetails OrderDetails { get; set; }
        [Required]
        public Guid PaymentMethodId { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        [Required]
        public Guid DeliveryMethodId { get; set; }
        [Required]
        public DeliveryMethod DeliveryMethod { get; set; }
        [Required]
        public List<OrderedProduct> OrderedProducts { get; set; }
        public string Status { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public DateTime WarrantyIsInForce { get; set; }
    }
}