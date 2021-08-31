using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Category : BaseDateTimeInfoEntry
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MinLength(5)]
        public string Name { get; set; }
        public string Image { get; set; }
        [Required, MaxLength(1000)]
        public string Description { get; set; }
    }
}