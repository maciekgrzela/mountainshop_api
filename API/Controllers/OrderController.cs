using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Complaint;
using Application.Complaint.Resources;
using Application.Order;
using Application.Order.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/orders")]
    public class OrderController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<OrderResource>>> GetAllAsync()
        {
            return await Mediator.Send(new GetAllOrders.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResource>> GetAsync(Guid id)
        {
            return await Mediator.Send(new GetOrder.Query{Id = id});
        }
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<OrderResource>>> GetOrdersForUserAsync(string userId)
        {
            return await Mediator.Send(new GetOrdersForUser.Query { Id = userId });
        }
        
        [HttpGet("last/for/user/{userId}")]
        public async Task<ActionResult<OrderForUserResource>> GetLastOrderForUserAsync(string userId)
        {
            return await Mediator.Send(new GetLastOrderForUser.Query{Id = userId});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateOrder.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPatch("{id}/paid")]
        public async Task<ActionResult> ChangeOrderStatusAsync(Guid id)
        {
            await Mediator.Send(new ChangeOrderStatus.Command {Id = id});
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeleteOrder.Command { Id = id });
            return NoContent();
        }

        [HttpGet("{id}/complaints")]
        public async Task<ActionResult<List<ComplaintResource>>> GetComplaintsForOrderAsync(Guid id)
        {
            return await Mediator.Send(new GetComplaintsForOrder.Query {Id = id});
        }
    }
}