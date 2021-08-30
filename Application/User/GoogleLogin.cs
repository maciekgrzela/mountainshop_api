using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User
{
    public class GoogleLogin
    {
        public class Query : IRequest<LoggedUserResource>
        {
            public string AccessToken { get; set; }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.AccessToken).NotEmpty();
            }
        }
        
        public class Handler : IRequestHandler<Query, LoggedUserResource>
        {
            private readonly UserManager<Domain.Models.User> _userManager;
            private readonly IWebTokenGenerator _webTokenGenerator;
            private readonly IGoogleAuthAccessor _googleAuthAccessor;
            private readonly IMapper _mapper;

            public Handler(UserManager<Domain.Models.User> userManager, IWebTokenGenerator webTokenGenerator, IGoogleAuthAccessor googleAuthAccessor, IMapper mapper)
            {
                _userManager = userManager;
                _webTokenGenerator = webTokenGenerator;
                _googleAuthAccessor = googleAuthAccessor;
                _mapper = mapper;
            }
            
            public async Task<LoggedUserResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var userAccountInfo = await _googleAuthAccessor.GoogleLogin(request.AccessToken);

                if (userAccountInfo == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono użytkownika dla podanego identyfikatora"});
                }

                var user = await _userManager.FindByNameAsync($"go_{userAccountInfo.Id}");

                if (user == null)
                {
                    user = new Domain.Models.User
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = userAccountInfo.FirstName,
                        LastName = userAccountInfo.LastName,
                        Email = userAccountInfo.Email,
                        UserName = $"go_{userAccountInfo.Id}",
                        Image = userAccountInfo.Image,
                        Role = "Customer",
                        Comments = new List<Domain.Models.Comment>()
                    };
                        
                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        throw new RestException(HttpStatusCode.Unauthorized,
                            new {info = "Użytkownik dla podanego loginu już istnieje"});
                    }

                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Customer"));
                    await _userManager.AddToRoleAsync(user, "Customer");
                }
                
                var loggedUser = _mapper.Map<Domain.Models.User, LoggedUserResource>(user);
                loggedUser.Token = _webTokenGenerator.CreateToken(user, "Customer");

                return loggedUser;
            }
        }
    }
}