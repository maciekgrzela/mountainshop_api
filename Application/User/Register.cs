using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Validators;
using AutoMapper;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User
{
    public class Register
    {
        public class Query : IRequest<LoggedUserResource>
        {
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            private string _role;

            public void setRole(string role)
            {
                _role = role;
            }

            public string getRole()
            {
                return _role;
            }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.UserName).NotEmpty();
                RuleFor(p => p.FirstName).NotEmpty().MaximumLength(150);
                RuleFor(p => p.LastName).NotEmpty().MaximumLength(200);
                RuleFor(p => p.PhoneNumber).NotEmpty().Length(11);
                RuleFor(p => p.Email).NotEmpty().EmailAddress();
                RuleFor(p => p.Password).Password();
            }
        }
        
        public class Handler : IRequestHandler<Query, LoggedUserResource>
        {
            private readonly UserManager<Domain.Models.User> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly IWebTokenGenerator _webTokenGenerator;
            private readonly IMapper _mapper;

            public Handler(UserManager<Domain.Models.User> userManager, RoleManager<IdentityRole> roleManager, IWebTokenGenerator webTokenGenerator, IMapper mapper)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _webTokenGenerator = webTokenGenerator;
                _mapper = mapper;
            }
            
            
            public async Task<LoggedUserResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var existing = await _userManager.FindByEmailAsync(request.Email);

                if (existing != null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized,
                        new {user = "Użytkownik dla podanego adresu e-mail już istnieje"});
                }

                existing = await _userManager.FindByNameAsync(request.UserName);

                if (existing != null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized,
                        new {user = "Użytkownik dla podanego loginu już istnieje"});
                }

                var user = new Domain.Models.User
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Role = request.getRole(),
                    Comments = new List<Comment>()
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    throw new RestException(HttpStatusCode.Unauthorized,
                        new {user = "Użytkownik dla podanego loginu już istnieje"});
                }

                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, request.getRole()));
                await _userManager.AddToRoleAsync(user, "Customer");

                var loggedUser = _mapper.Map<Domain.Models.User, LoggedUserResource>(user);

                loggedUser.Token = _webTokenGenerator.CreateToken(user, request.getRole());

                return loggedUser;
            }
        }
    }
}