using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Complaint;
using Application.Complaint.Resources;
using Application.Order;
using Application.Order.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<OrderResource>>> GetAllAsync()
        {
            return await _mediator.Send(new GetAllOrders.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResource>> GetAsync(Guid id)
        {
            return await _mediator.Send(new GetOrder.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateOrder.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateOrder.Command data)
        {
            data.SetId(id);
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteOrder.Command { Id = id });
            return NoContent();
        }

        [HttpGet("{id}/complaints")]
        public async Task<ActionResult<List<ComplaintResource>>> GetComplaintsForOrderAsync(Guid id)
        {
            return await _mediator.Send(new GetComplaintsForOrder.Query {Id = id});
        }
    }
}