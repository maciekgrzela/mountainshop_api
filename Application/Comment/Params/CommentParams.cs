using System;

namespace Application.Comment.Params
{
    public class CommentParams : PagingParams
    {
        public string UserId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}