using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ContactRequest : BaseDateTimeInfoEntry
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        [Required, MaxLength(100)]
        public string Email { get; set; }
        [Required, MaxLength(1000)]
        public string Content { get; set; }
    }
}