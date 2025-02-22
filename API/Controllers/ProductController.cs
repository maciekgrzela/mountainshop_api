﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application;
using Application.Category;
using Application.Comment;
using Application.Comment.Resources;
using Application.Product;
using Application.Product.Params;
using Application.Product.Resources;
using Application.ProductsProperty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/products")]
    public class ProductController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedList<ProductResource>>> GetAllProductsAsync([FromQuery] ProductParams queryParams)
        {
            var products = await Mediator.Send(new GetAllProducts.Query {QueryParams = queryParams});
            return HandlePaginationHeader(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResource>> GetProduct(Guid id)
        {
            return await Mediator.Send(new GetProduct.Query {Id = id});
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> CreateProductAsync(CreateProduct.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPost("photo/upload")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> UploadPhotoAsync([FromForm] UploadProductsImage.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPatch("{productsId}/property/{propertyId}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> AddPropertyToProduct(Guid productsId, Guid propertyId, AddPropertyToProduct.Request data)
        {
            await Mediator.Send(new AddPropertyToProduct.Command {ProductId = productsId, PropertyId = propertyId, Value = data.Value});
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> UpdateProductAsync(UpdateProduct.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPatch("{id}/sale/add")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> SetProductsSaleAsync(Guid id, SetProductsSale.Command data)
        {
            data.SetId(id);
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> DeleteProductAsync(Guid id)
        {
            await Mediator.Send(new DeleteProduct.Command {Id = id});
            return NoContent();
        }

        [HttpDelete("{productId}/property/{propertyId}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> DeleteProductsPropertyValueForProductAsync(Guid productId, Guid propertyId)
        {
            await Mediator.Send(new DeletePropertyFromProduct.Command {ProductId = productId, PropertyId = propertyId});
            return NoContent();
        }
        

        [AllowAnonymous]
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<List<CommentResource>>> GetCommentsForProductAsync(Guid id)
        {
            return await Mediator.Send(new GetCommentsForProduct.Query {Id = id});
        }
        
        [AllowAnonymous]
        [HttpGet("{id}/properties")]
        public async Task<ActionResult<List<PropertyValueForProductResource>>> GetPropertiesForProductAsync(Guid id)
        {
            return await Mediator.Send(new GetPropertiesForProduct.Query {Id = id});
        }
    }
}