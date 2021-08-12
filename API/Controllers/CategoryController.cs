using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Category;
using Application.Product;
using Application.Product.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryResource>>> GetAllAsync()
        {
            return await _mediator.Send(new GetAllCategories.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResource>> GetAsync(Guid id)
        {
            return await _mediator.Send(new GetCategory.Query{Id = id});
        }
        
        [HttpGet("{id}/products")]
        public async Task<ActionResult<List<ProductResource>>> GetProductsForCategoryAsync(Guid id)
        {
            return await _mediator.Send(new GetProductsForCategory.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateCategory.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateCategory.Command data)
        {
            data.SetId(id);
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteCategory.Command { Id = id });
            return NoContent();
        }
    }
}