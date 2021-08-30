using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;

namespace Application.User
{
    public class GetCurrentUser
    {
        public class Query : IRequest<LoggedUserResource> { }
        
        public class Handler : IRequestHandler<Query, LoggedUserResource>
        {
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly UserManager<Domain.Models.User> _userManager;

            public Handler(IMapper mapper, IUserAccessor userAccessor, UserManager<Domain.Models.User> userManager)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _userManager = userManager;
            }
            
            public async Task<LoggedUserResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetUserName());

                if (user == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono użytkownika dla podanego identyfikatora"});
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                
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
                };
            }
        }
    }
}