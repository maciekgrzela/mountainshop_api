using System;
using System.Threading.Tasks;
using Application.ProductsCategories;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/products-categories")]
    public class ProductsCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("assign/product/{productId}/to/categories")]
        public async Task<ActionResult> AssignProductToCategoriesAsync(Guid productId)
        {
            var data = new AssignProductToCategories.Command();
            data.ProductId = productId;
            await _mediator.Send(data);
            return NoContent();
        }
        
        [HttpPost("add/product/{productId}/to/category/{categoryId}")]
        public async Task<ActionResult> AssignProductToCategoryAsync(Guid productId, Guid categoryId)
        {
            var data = new AssignProductToCategory.Command {ProductId = productId, CategoryId = categoryId};
            await _mediator.Send(data);
            return NoContent();
        }
        
        [HttpPost("remove/products/{productId}/categories")]
        public async Task<ActionResult> RemoveProductsCategoriesAsync(Guid productId)
        {
            await _mediator.Send(new RemoveProductsCategories.Command {ProductId = productId});
            return NoContent();
        }
    }
}