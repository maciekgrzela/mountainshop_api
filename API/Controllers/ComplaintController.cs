using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Comment;
using Application.Complaint;
using Application.Complaint.Params;
using Application.Complaint.Resources;
using Application.Compliant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/complaints")]
    public class ComplaintController : BaseController
    {
        [HttpGet]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ComplaintParams queryParams)
        {
            var complaints = await Mediator.Send(new GetAllComplaints.Query {QueryParams = queryParams});
            return HandlePaginationHeader(complaints);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintResource>> GetAsync(Guid id)
        {
            return await Mediator.Send(new GetComplaint.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateCompliant.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateCompliant.Command data)
        {
            data.SetId(id);
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeleteComment.Command { Id = id });
            return NoContent();
        }
    }
}