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
        [HttpPost("create/session/{userId}")]
        public async Task<ActionResult> CreateSessionAsync(string userId)
        {
            var sessionUrl = await Mediator.Send(new CreateCheckoutSession.Command {Id = userId});
            Response.Headers.Add("Location", sessionUrl);
            return new StatusCodeResult(303);
        }
    }
}