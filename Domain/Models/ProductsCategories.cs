using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ProductsCategories
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Product Product { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Category Category { get; set; }
    }
}