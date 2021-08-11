using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DeliveryMethod;
using Application.DeliveryMethod.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/delivery")]
    public class DeliveryMethodController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeliveryMethodController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<DeliveryMethodResource>>> GetAllAsync()
        {
            return await _mediator.Send(new GetAllDeliveryMethods.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryMethodResource>> GetAsync(Guid id)
        {
            return await _mediator.Send(new GetDeliveryMethod.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateDeliveryMethod.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateDeliveryMethod.Command data)
        {
            data.SetId(id);
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteDeliveryMethod.Command { Id = id });
            return NoContent();
        }
        
    }
}