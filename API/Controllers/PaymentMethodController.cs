using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.PaymentMethod;
using Application.PaymentMethod.Params;
using Application.PaymentMethod.Resources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/payment")]
    public class PaymentMethodController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaymentMethodParams queryParams)
        {
            var payments = await Mediator.Send(new GetAllPaymentMethods.Query{QueryParams = queryParams });
            return HandlePaginationHeader(payments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentMethodResource>> GetAsync(Guid id)
        {
            return await Mediator.Send(new GetPaymentMethod.Query{Id = id});
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> CreateAsync(CreatePaymentMethod.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }
        
        [HttpPatch("{paymentId}/assign/delivery/{deliveryId}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> AssignPaymentToDeliveryAsync(Guid paymentId, Guid deliveryId)
        {
            await Mediator.Send(new AssignPaymentToDelivery.Command {PaymentId = paymentId, DeliveryId = deliveryId});
            return NoContent();
        }
        
        [HttpPatch("{paymentId}/remove/delivery/{deliveryId}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> RemovePaymentFromDeliveryAsync(Guid paymentId, Guid deliveryId)
        {
            await Mediator.Send(new AssignPaymentToDelivery.Command {PaymentId = paymentId, DeliveryId = deliveryId});
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdatePaymentMethod.Command data)
        {
            data.SetId(id);
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeletePaymentMethod.Command { Id = id });
            return NoContent();
        }
    }
}