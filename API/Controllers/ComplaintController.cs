using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Comment;
using Application.Complaint;
using Application.Complaint.Resources;
using Application.Compliant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/complaints")]
    public class ComplaintController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ComplaintController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult<List<ComplaintResource>>> GetAllAsync()
        {
            return await _mediator.Send(new GetAllCompliants.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintResource>> GetAsync(Guid id)
        {
            return await _mediator.Send(new GetComplaint.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateCompliant.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateCompliant.Command data)
        {
            data.SetId(id);
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteComment.Command { Id = id });
            return NoContent();
        }
    }
}