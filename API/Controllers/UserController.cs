using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Comment;
using Application.Comment.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<List<CommentResource>>> GetCommentsForUserAsync(string id)
        {
            return await Mediator.Send(new GetCommentsForUser.Query {Id = id});
        }
    }
}