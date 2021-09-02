using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.User
{
    public class Login
    {
        public class Query : IRequest<LoggedUserResource>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Email).NotEmpty().EmailAddress();
                RuleFor(p => p.Password).NotEmpty();
            }
        }
        
        public class Handler : IRequestHandler<Query, LoggedUserResource>
        {
            private readonly SignInManager<Domain.Models.User> _signInManager;
            private readonly UserManager<Domain.Models.User> _userManager;
            private readonly IWebTokenGenerator _webTokenGenerator;

            public Handler(SignInManager<Domain.Models.User> signInManager, UserManager<Domain.Models.User> userManager, IWebTokenGenerator webTokenGenerator)
            {
                _signInManager = signInManager;
                _userManager = userManager;
                _webTokenGenerator = webTokenGenerator;
            }
            
            public async Task<LoggedUserResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.Users.Where(p => p.Email == request.Email && !p.UserName.Contains("fb") && !p.UserName.Contains("go")).FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (user == null)
                {
                    throw new RestException(HandlerResponse.ClientIsNotAuthorized, new
                    {
                        user = "Użytkownik nie istnieje!"
                    });
                }

                var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                var userRoles = await _userManager.GetRolesAsync(user);

                if (userRoles.Count == 0)
                {
                    throw new RestException(HandlerResponse.ClientIsNotAuthorized, new { info = "Użytkownik nie posiada przypisanej roli"});
                }

                if (!checkPassword.Succeeded)
                {
                    throw new RestException(HandlerResponse.ClientIsNotAuthorized, new {info = "Niepoprawne dane logowania"});
                }

                return new LoggedUserResource
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Image = user.Image,
                    Role = userRoles[0],
                    Token = _webTokenGenerator.CreateToken(user, userRoles[0])
                };
            }
        }
    }
}