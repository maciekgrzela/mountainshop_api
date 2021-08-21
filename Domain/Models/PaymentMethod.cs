using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class PaymentMethod
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MinLength(10)]
        public string Name { get; set; }
        [Required, Range(0 ,double.MaxValue)]
        public double Price { get; set; }
        [Required]
        public List<Order> Orders { get; set; }
    }
}