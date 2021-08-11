using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Complaint;
using Application.Complaint.Resources;
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

        [HttpGet("{id}/complaints")]
        public async Task<ActionResult<List<ComplaintResource>>> GetComplaintsForOrderAsync(Guid id)
        {
            return await _mediator.Send(new GetComplaintsForOrder.Query {Id = id});
        }
    }
}