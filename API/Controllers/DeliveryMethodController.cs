using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DeliveryMethod;
using Application.DeliveryMethod.Params;
using Application.DeliveryMethod.Resources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/delivery")]
    public class DeliveryMethodController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] DeliveryMethodParams queryParams)
        {
            var deliveryMethods = await Mediator.Send(new GetAllDeliveryMethods.Query { QueryParams = queryParams });
            return HandlePaginationHeader(deliveryMethods);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryMethodResource>> GetAsync(Guid id)
        {
            return await Mediator.Send(new GetDeliveryMethod.Query{Id = id});
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> CreateAsync(CreateDeliveryMethod.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateDeliveryMethod.Command data)
        {
            data.SetId(id);
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeleteDeliveryMethod.Command { Id = id });
            return NoContent();
        }
        
    }
}