using System;

namespace Application.Product.Resources
{
    public class CommentForProductResource
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public UserForProductsCommentResource User { get; set; }
        public DateTime Created { get; set; }
        public double Rate { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}