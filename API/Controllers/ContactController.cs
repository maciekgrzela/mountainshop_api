using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Contact;
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
        public async Task<List<ContactRequest>> GetAllContactRequestsAsync()
        {
            return await Mediator.Send(new GetAllContactRequests.Query());
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