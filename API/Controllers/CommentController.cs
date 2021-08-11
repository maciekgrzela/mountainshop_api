using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application.Comment;
using Application.Comment.Resources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<CommentResource>>> GetAllAsync()
        {
            return await _mediator.Send(new GetAllComments.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentResource>> GetAsync(Guid id)
        {
            return await _mediator.Send(new GetComment.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateComment.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpPatch("{id}/like")]
        public async Task<ActionResult> LikeCommentAsync(Guid id)
        {
            await _mediator.Send(new ModifyCommentsPopularity.Command {Id = id, Vote = "like"});
            return NoContent();
        }
        
        [HttpPatch("{id}/dislike")]
        public async Task<ActionResult> DislikeCommentAsync(Guid id)
        {
            await _mediator.Send(new ModifyCommentsPopularity.Command {Id = id, Vote = "dislike"});
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateComment.Command data)
        {
            data.SetId(id);
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteComment.Command { Id = id });
            return NoContent();
        }
    }
}