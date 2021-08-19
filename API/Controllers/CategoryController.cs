using System;
using System.Threading.Tasks;
using Application;
using Application.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/categories")]
    public class CategoryController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagingParams queryParams)
        {
            var categories = await Mediator.Send(new GetAllCategories.Query{ QueryParams = queryParams});
            return HandlePaginationHeader(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResource>> GetAsync(Guid id)
        {
            return await Mediator.Send(new GetCategory.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromForm] CreateCategory.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateCategory.Command data)
        {
            data.SetId(id);
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeleteCategory.Command { Id = id });
            return NoContent();
        }
    }
}