using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ProductsPropertyValue
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Product Product { get; set; }
        [Required]
        public Guid ProductsPropertyId { get; set; }
        [Required]
        public ProductsProperty ProductsProperty { get; set; }
        [Required, MinLength(1)]
        public string Value { get; set; }
    }
}