using System;
using System.Threading.Tasks;
using Application.Checkout;
using Application.Checkout.Resources;
using AutoMapper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/checkout")]
    [AllowAnonymous]
    public class CheckoutController : BaseController
    {
        [HttpPost("create/session/{userId}")]
        public async Task<IActionResult> CreateSessionAsync(string userId)
        {
            var sessionUrl = await Mediator.Send(new CreateCheckoutSession.Command {Id = userId});
            Response.Headers.Add("Location", sessionUrl);
            return new StatusCodeResult(303);
        }

        [HttpGet("session/{id}")]
        public async Task<ActionResult<CheckoutSessionResource>> GetSessionAsync(string id)
        {
            return await Mediator.Send(new GetCheckoutSession.Query{Id = id});
        }
    }
}