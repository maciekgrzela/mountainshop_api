using System.Threading.Tasks;
using Application.Checkout;
using AutoMapper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/checkout")]
    public class CheckoutController : BaseController
    {
        [HttpPost("create/session")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateSessionAsync()
        {
            var sessionUrl = await Mediator.Send(new CreateCheckoutSession.Command());
            Response.Headers.Add("Location", sessionUrl);
            return new StatusCodeResult(303);
        }
    }
}