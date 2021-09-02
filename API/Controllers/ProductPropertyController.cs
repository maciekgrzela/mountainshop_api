using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.ProductsProperty;
using Application.ProductsProperty.Params;
using Application.ProductsProperty.Resources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/product/properties")]
    public class ProductPropertyController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductsPropertyParams queryParams)
        {
            var productsProperties = await Mediator.Send(new GetAllProductsProperties.Query{ QueryParams = queryParams});
            return HandlePaginationHeader(productsProperties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductsPropertyResource>> GetAsync(Guid id)
        {
            return await Mediator.Send(new GetProductsProperty.Query{Id = id});
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> CreateAsync(CreateProductsProperty.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateProductsProperty.Command data)
        {
            data.SetId(id);
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeleteProductsProperty.Command { Id = id });
            return NoContent();
        }
    }
}