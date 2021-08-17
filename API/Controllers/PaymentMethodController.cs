using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.PaymentMethod;
using Application.PaymentMethod.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentMethodController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<PaymentMethodResource>>> GetAllAsync()
        {
            return await _mediator.Send(new GetAllPaymentMethods.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentMethodResource>> GetAsync(Guid id)
        {
            return await _mediator.Send(new GetPaymentMethod.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreatePaymentMethod.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdatePaymentMethod.Command data)
        {
            data.SetId(id);
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeletePaymentMethod.Command { Id = id });
            return NoContent();
        }
    }
}