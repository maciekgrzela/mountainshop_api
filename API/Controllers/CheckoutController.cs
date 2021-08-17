using System.Threading.Tasks;
using Application.Checkout;
using AutoMapper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CheckoutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create/session")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateSessionAsync()
        {
            var sessionUrl = await _mediator.Send(new CreateCheckoutSession.Command());
            Response.Headers.Add("Location", sessionUrl);
            return new StatusCodeResult(303);
        }
    }
}