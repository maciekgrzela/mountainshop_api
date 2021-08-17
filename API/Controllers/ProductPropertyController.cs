using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.ProductsProperty;
using Application.ProductsProperty.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/product/properties")]
    public class ProductPropertyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductPropertyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<ProductsPropertyResource>>> GetAllAsync()
        {
            return await _mediator.Send(new GetAllProductsProperties.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductsPropertyResource>> GetAsync(Guid id)
        {
            return await _mediator.Send(new GetProductsProperty.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateProductsProperty.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateProductsProperty.Command data)
        {
            data.SetId(id);
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteProductsProperty.Command { Id = id });
            return NoContent();
        }
    }
}