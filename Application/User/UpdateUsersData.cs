using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;

namespace Application.User
{
    public class UpdateUsersData
    {
        public class Command : IRequest
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
        }
        
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IUserAccessor _userAccessor;
            private readonly UserManager<Domain.Models.User> _userManager;

            public Handler(DataContext context, IUnitOfWork unitOfWork, IUserAccessor userAccessor, UserManager<Domain.Models.User> userManager)
            {
                _context = context;
                _unitOfWork = unitOfWork;
                _userAccessor = userAccessor;
                _userManager = userManager;
            }
            
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingUser = await _userManager.FindByNameAsync(_userAccessor.GetUserName());

                if (existingUser == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized,
                        new {info = "Zaloguj się przed wykonaniem powyższej operacji"});
                }

                existingUser.Email = request.Email;
                existingUser.FirstName = request.FirstName;
                existingUser.LastName = request.LastName;
                existingUser.PhoneNumber = request.PhoneNumber;

                await _userManager.UpdateAsync(existingUser);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}