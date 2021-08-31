using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class DeliveryMethod : BaseDateTimeInfoEntry
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MinLength(5)]
        public string Name { get; set; }
        [Required, Range(0, double.MaxValue)]
        public double Price { get; set; }
        public List<Order> Orders { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
    }
}