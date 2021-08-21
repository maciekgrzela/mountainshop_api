using System;

namespace Domain.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public DateTime Created { get; set; }
        public double Rate { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}