using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;

namespace Application.User
{
    public class FacebookLogin
    {
        public class Query : IRequest<LoggedUserResource>
        {
            public string Email { get; set; }
            public string Password { get; set; }
            private string _role;

            public void setRole(string role)
            {
                _role = role;
            }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Email).NotEmpty().EmailAddress();
                RuleFor(p => p.Password).NotEmpty().Password();
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
                throw new NotImplementedException();
            }
        }
    }
}