using System;

namespace Application.Comment.Resources
{
    public class CommentResource
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public UserForCommentsResource User { get; set; }
        public ProductForCommentsResource Product { get; set; }
        public DateTime Created { get; set; }
        public double Rate { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }

    public class ProductForCommentsResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UserForCommentsResource
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}