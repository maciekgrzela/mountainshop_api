using System;

namespace Application.Comment.Params
{
    public class CommentParams : PagingParams
    {
        public string UserFilter { get; set; }
        public bool? UserSort { get; set; }
        public Guid? ProductFilter { get; set; }
        public bool? ProductSort { get; set; }
        public DateTime? CreatedSort { get; set; }
    }
}