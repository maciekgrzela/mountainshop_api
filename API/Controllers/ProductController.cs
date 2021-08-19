using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Category;
using Application.Comment;
using Application.Comment.Resources;
using Application.Product;
using Application.Product.Params;
using Application.Product.Resources;
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
        public async Task<ActionResult<List<ProductResource>>> GetAllProductsAsync([FromQuery] ProductParams queryParams)
        {
            var products = await Mediator.Send(new GetAllProducts.Query {QueryParams = queryParams});
            return HandlePaginationHeader(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResource>> GetProduct(Guid id)
        {
            return await Mediator.Send(new GetProduct.Query {Id = id});
        }
        
        [HttpPost]
        public async Task<ActionResult> CreateProductAsync(CreateProduct.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }
        
        [HttpPost("photo/upload")]
        public async Task<ActionResult> UploadPhotoAsync([FromForm] UploadProductsImage.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProductAsync(UpdateProduct.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductAsync(Guid id)
        {
            await Mediator.Send(new DeleteProduct.Command {Id = id});
            return NoContent();
        }

        [HttpDelete("{productId}/property/{propertyId}")]
        public async Task<ActionResult> DeleteProductsPropertyValueForProductAsync(Guid productId, Guid propertyId)
        {
            await Mediator.Send(new DeletePropertyFromProduct.Command {ProductId = productId, PropertyId = propertyId});
            return NoContent();
        }
        

        [HttpGet("{id}/comments")]
        public async Task<ActionResult<List<CommentResource>>> GetCommentsForProductAsync(Guid id)
        {
            return await Mediator.Send(new GetCommentsForProduct.Query {Id = id});
        }
    }
}