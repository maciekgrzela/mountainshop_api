using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using Application.Validators;
using AutoMapper;
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
            private readonly IFacebookGraphApiAccessor _facebookGraphApiAccessor;
            private readonly IMapper _mapper;

            public Handler(UserManager<Domain.Models.User> userManager, IWebTokenGenerator webTokenGenerator, IFacebookGraphApiAccessor facebookGraphApiAccessor, IMapper mapper)
            {
                _userManager = userManager;
                _webTokenGenerator = webTokenGenerator;
                _facebookGraphApiAccessor = facebookGraphApiAccessor;
                _mapper = mapper;
            }
            
            public async Task<LoggedUserResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var userAccountInfo = await _facebookGraphApiAccessor.FacebookLogin(request.AccessToken);

                if (userAccountInfo == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono użytkownika dla podanego identyfikatora"});
                }

                var user = await _userManager.FindByNameAsync($"fb_{userAccountInfo.Id}");

                if (user == null)
                {
                    user = new Domain.Models.User
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = userAccountInfo.Name.Split(' ', 2)[0],
                        LastName = userAccountInfo.Name.Split(' ', 2)[1],
                        Email = userAccountInfo.Email,
                        UserName = $"fb_{userAccountInfo.Id}",
                        Image = userAccountInfo.Picture.Data.Url,
                        Role = "Customer",
                        Comments = new List<Domain.Models.Comment>()
                    };
                        
                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        throw new RestException(HandlerResponse.ClientIsNotAuthorized,
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