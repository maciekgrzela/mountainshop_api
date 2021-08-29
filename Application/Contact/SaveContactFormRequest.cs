using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using MediatR;
using Persistence.Context;

namespace Application.Contact
{
    public class SaveContactFormRequest
    {
        public class Command : IRequest
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Content { get; set; }
        }
        
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(DataContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }
            
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var contactRequest = new ContactRequest
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Content = request.Content,
                    Email = request.Email,
                    Created = DateTime.Now
                };

                await _context.ContactRequests.AddAsync(contactRequest, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}