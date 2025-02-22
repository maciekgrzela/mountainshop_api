﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Double;

namespace Domain.Models
{
    public class Product : BaseDateTimeInfoEntry
    {
        [Key]
        public Guid Id { get; set; }
        [MinLength(5), MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public string Image { get; set; }
        public List<ProductsPropertyValue> ProductsPropertyValues { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int AmountInStorage { get; set; }
        [Required, Range(0, MaxValue)]
        public double NetPrice { get; set; }
        [Required, Range(0, 100)]
        public double PercentageTax { get; set; }
        [Required, Range(0, MaxValue)]
        public double GrossPrice { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int MinimalOrderedAmount { get; set; }
        public double? PercentageSale { get; set; }
        public string Gender { get; set; }
        [Required]
        public Guid ProducerId { get; set; }
        [Required]
        public Producer Producer { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Category Category { get; set; }
        public List<Comment> Comments { get; set; }
    }
}