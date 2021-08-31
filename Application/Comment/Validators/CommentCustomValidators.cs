using System.Collections.Generic;

namespace Application.Comment.Validators
{
    public static class CommentCustomValidators
    {
        public static bool ValidVote(string vote)
        {
            var validList = new List<string>
            {
                "like", "dislike"
            };

            return validList.Contains(vote);
        }
    }
}