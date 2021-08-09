using System.Threading.Tasks;
using Application.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoggedUserResource>> LoginAsync(Login.Query query)
        {
            return await _mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("register/customer")]
        public async Task<ActionResult<LoggedUserResource>> RegisterCustomerAsync(Register.Query query)
        {
            query.setRole("Customer");
            return await _mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("register/facebook")]
        public async Task<ActionResult<LoggedUserResource>> RegisterCustomerWithFacebook(FacebookLogin.Query query)
        {
            query.setRole("Customer");
            return await _mediator.Send(query);
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPost("register/owner")]
        public async Task<ActionResult<LoggedUserResource>> RegisterOwnerAsync(Register.Query query)
        {
            query.setRole("Owner");
            return await _mediator.Send(query);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register/admin")]
        public async Task<ActionResult<LoggedUserResource>> RegisterAdminAsync(Register.Query query)
        {
            query.setRole("Admin");
            return await _mediator.Send(query);
        }
    }
}