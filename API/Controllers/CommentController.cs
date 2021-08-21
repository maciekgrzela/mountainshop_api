using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application;
using Application.Comment;
using Application.Comment.Params;
using Application.Comment.Resources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/comments")]
    public class CommentController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] CommentParams queryParams)
        {
            var comments = await Mediator.Send(new GetAllComments.Query{ QueryParams = queryParams });
            return HandlePaginationHeader(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentResource>> GetAsync(Guid id)
        {
            return await Mediator.Send(new GetComment.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateComment.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPatch("{id}/like")]
        public async Task<ActionResult> LikeCommentAsync(Guid id)
        {
            await Mediator.Send(new ModifyCommentsPopularity.Command {Id = id, Vote = "like"});
            return NoContent();
        }
        
        [AllowAnonymous]
        [HttpPatch("{id}/like/cancel")]
        public async Task<ActionResult> CancelLikeCommentAsync(Guid id)
        {
            await Mediator.Send(new CancelCommentsVote.Command {Id = id, Vote = "like"});
            return NoContent();
        }
        
        [AllowAnonymous]
        [HttpPatch("{id}/dislike")]
        public async Task<ActionResult> DislikeCommentAsync(Guid id)
        {
            await Mediator.Send(new ModifyCommentsPopularity.Command {Id = id, Vote = "dislike"});
            return NoContent();
        }
        
        [AllowAnonymous]
        [HttpPatch("{id}/dislike/cancel")]
        public async Task<ActionResult> CancelDislikeCommentAsync(Guid id)
        {
            await Mediator.Send(new CancelCommentsVote.Command {Id = id, Vote = "dislike"});
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateComment.Command data)
        {
            data.SetId(id);
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeleteComment.Command { Id = id });
            return NoContent();
        }
    }
}