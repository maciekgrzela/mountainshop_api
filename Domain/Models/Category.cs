using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MinLength(5)]
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
    }
}