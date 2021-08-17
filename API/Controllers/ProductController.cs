﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Category;
using Application.Comment;
using Application.Comment.Resources;
using Application.Product;
using Application.Product.Resources;
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

        [HttpGet]
        public async Task<ActionResult<List<ProductResource>>> GetAllProductsAsync()
        {
            return await _mediator.Send(new GetAllProducts.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResource>> GetProduct(Guid id)
        {
            return await _mediator.Send(new GetProduct.Query {Id = id});
        }
        
        [HttpPost]
        public async Task<ActionResult> CreateProductAsync(CreateProduct.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProductAsync(UpdateProduct.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductAsync(Guid id)
        {
            await _mediator.Send(new DeleteProduct.Command {Id = id});
            return NoContent();
        }

        [HttpDelete("{productId}/property/{propertyId}")]
        public async Task<ActionResult> DeleteProductsPropertyValueForProductAsync(Guid productId, Guid propertyId)
        {
            await _mediator.Send(new DeletePropertyFromProduct.Command {ProductId = productId, PropertyId = propertyId});
            return NoContent();
        }
        

        [HttpGet("{id}/comments")]
        public async Task<ActionResult<List<CommentResource>>> GetCommentsForProductAsync(Guid id)
        {
            return await _mediator.Send(new GetCommentsForProduct.Query {Id = id});
        }
        
        [HttpGet("{id}/categories")]
        public async Task<ActionResult<List<CategoryResource>>> GetCategoriesForProductAsync(Guid id)
        {
            return await _mediator.Send(new GetCategoriesForProduct.Query {Id = id});
        }
    }
}