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
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}/comments")]
        public async Task<ActionResult<List<CommentResource>>> GetCommentsForProductAsync(Guid id)
        {
            return await _mediator.Send(new GetCommentsForProduct.Query {Id = id});
        }
    }
}