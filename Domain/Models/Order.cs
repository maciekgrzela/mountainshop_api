using System;
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
        [Required, Range(0, double.MaxValue)]
        public double NetPrice { get; set; }
        [Required, Range(0, 100)]
        public double PercentageTax { get; set; }
        [Required, Range(0, double.MaxValue)]
        public double GrossPrice { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int TotalAmount { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public DateTime WarrantyIsInForce { get; set; }
    }
}