using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Compliant
{
    public class CreateCompliant
    {
        public class Command : IRequest
        {
            public string Abbreviation { get; set; }
            public string Description { get; set; }
            public Guid OrderId { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Abbreviation).NotEmpty();
                RuleFor(p => p.OrderId).NotEmpty();
                RuleFor(p => p.Description).NotEmpty();
            }
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
                var existingOrder = await _context.Orders.FindAsync(request.OrderId);

                if (existingOrder == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono kategorii dla podanego identyfikatora"});
                }

                var complaint = new Domain.Models.Complaint()
                {
                    Id = Guid.NewGuid(),
                    Abbreviation = request.Abbreviation,
                    Description = request.Description,
                    Order = existingOrder
                };

                await _context.Complaints.AddAsync(complaint, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}