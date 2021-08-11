using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Comment;
using Application.Comment.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}/comments")]
        public async Task<ActionResult<List<CommentResource>>> GetCommentsForUserAsync(string id)
        {
            return await _mediator.Send(new GetCommentsForUser.Query {Id = id});
        }
    }
}