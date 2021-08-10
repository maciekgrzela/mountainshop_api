using System;

namespace Application.Comment.Resources
{
    public class CommentResource
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public UserForCommentsResource User { get; set; }
        public ProductForCommentsResource Product { get; set; }
        public DateTime Created { get; set; }
        public double Rate { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}