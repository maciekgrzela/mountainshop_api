using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using Application.Validators;
using AutoMapper;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            public void SetRole(string role)
            {
                _role = role;
            }

            public string GetRole()
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
                var existingEmail = await _userManager.Users.Where(p => p.Email == request.Email).ToListAsync(cancellationToken: cancellationToken);

                if (existingEmail.Count > 0)
                {
                    throw new RestException(HandlerResponse.ClientIsNotAuthorized,
                        new {info = "Użytkownik dla podanego adresu e-mail już istnieje"});
                }

                var existing = await _userManager.FindByNameAsync(request.UserName);

                if (existing != null)
                {
                    throw new RestException(HandlerResponse.ClientIsNotAuthorized,
                        new {info = "Użytkownik dla podanego loginu już istnieje"});
                }

                var user = new Domain.Models.User
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Role = request.GetRole(),
                    Comments = new List<Domain.Models.Comment>()
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    throw new RestException(HandlerResponse.ClientIsNotAuthorized,
                        new {info = "Użytkownik dla podanego loginu już istnieje"});
                }

                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, request.GetRole()));
                await _userManager.AddToRoleAsync(user, "Customer");

                var loggedUser = _mapper.Map<Domain.Models.User, LoggedUserResource>(user);

                loggedUser.Token = _webTokenGenerator.CreateToken(user, request.GetRole());

                return loggedUser;
            }
        }
    }
}