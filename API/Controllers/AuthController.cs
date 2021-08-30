using System.Threading.Tasks;
using Application.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        [HttpGet("login/current")]
        public async Task<ActionResult<LoggedUserResource>> GetCurrentUserAsync()
        {
            return await Mediator.Send(new GetCurrentUser.Query());
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoggedUserResource>> LoginAsync(Login.Query query)
        {
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("register/customer")]
        public async Task<ActionResult<LoggedUserResource>> RegisterCustomerAsync(Register.Query query)
        {
            query.setRole("Customer");
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("register/facebook")]
        public async Task<ActionResult<LoggedUserResource>> RegisterCustomerWithFacebook(FacebookLogin.Query query)
        {
            return await Mediator.Send(query);
        }
        
        [AllowAnonymous]
        [HttpPost("register/google")]
        public async Task<ActionResult<LoggedUserResource>> RegisterCustomerWithGoogle(GoogleLogin.Query query)
        {
            return await Mediator.Send(query);
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPost("register/owner")]
        public async Task<ActionResult<LoggedUserResource>> RegisterOwnerAsync(Register.Query query)
        {
            query.setRole("Owner");
            return await Mediator.Send(query);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register/admin")]
        public async Task<ActionResult<LoggedUserResource>> RegisterAdminAsync(Register.Query query)
        {
            query.setRole("Admin");
            return await Mediator.Send(query);
        }

        [HttpPut("update/data")]
        public async Task<ActionResult> UpdateMyDataAsync(UpdateUsersData.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }
    }
}