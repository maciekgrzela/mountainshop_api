using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.DeliveryMethod
{
    public class DeleteDeliveryMethod
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
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
                var existingDeliveryMethod = await _context.DeliveryMethods.FindAsync(request.Id);

                if (existingDeliveryMethod == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono metody dostawy dla podanego identyfikatora"});
                }

                _context.DeliveryMethods.Remove(existingDeliveryMethod);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}