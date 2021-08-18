using System;
using System.Threading.Tasks;
using Application.ProductsCategories;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/products-categories")]
    public class ProductsCategoryController : BaseController
    {
        [HttpPost("assign/product/{productId}/to/categories")]
        public async Task<ActionResult> AssignProductToCategoriesAsync(Guid productId)
        {
            var data = new AssignProductToCategories.Command();
            data.ProductId = productId;
            await Mediator.Send(data);
            return NoContent();
        }
        
        [HttpPost("add/product/{productId}/to/category/{categoryId}")]
        public async Task<ActionResult> AssignProductToCategoryAsync(Guid productId, Guid categoryId)
        {
            var data = new AssignProductToCategory.Command {ProductId = productId, CategoryId = categoryId};
            await Mediator.Send(data);
            return NoContent();
        }
        
        [HttpPost("remove/products/{productId}/categories")]
        public async Task<ActionResult> RemoveProductsCategoriesAsync(Guid productId)
        {
            await Mediator.Send(new RemoveProductsCategories.Command {ProductId = productId});
            return NoContent();
        }
    }
}