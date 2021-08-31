using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Comment : BaseDateTimeInfoEntry
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MaxLength(150)]
        public string Title { get; set; }
        [Required, MaxLength(1000)]
        public string Content { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        [Required, Range(0, 5)]
        public double Rate { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Likes { get; set; }
        [Required, Range(0, int.MinValue)]
        public int Dislikes { get; set; }
    }
}