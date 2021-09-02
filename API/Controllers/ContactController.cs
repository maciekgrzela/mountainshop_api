using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contact;
using Application.Contact.Params;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/contact/requests")]
    public class ContactController : BaseController
    {

        [HttpGet]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> GetAllContactRequestsAsync([FromQuery] ContactRequestParams queryParams)
        {
            var contact = await Mediator.Send(new GetAllContactRequests.Query {QueryParams = queryParams});
            return HandlePaginationHeader(contact);
        }

        [HttpPost("send")]
        [AllowAnonymous]
        public async Task<ActionResult> SaveContactFormRequestAsync(SaveContactFormRequest.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }
    }
}