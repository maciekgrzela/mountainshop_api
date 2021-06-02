using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Complaint
    {
        [Key]
        public Guid Id { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Number { get; set; }
        [Required, MinLength(30)]
        public string Abbreviation { get; set; }
        [Required, MinLength(200)]
        public string Description { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public Order Order { get; set; }
    }
}