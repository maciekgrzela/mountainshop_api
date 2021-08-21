using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.PaymentMethod
{
    public class UpdatePaymentMethod
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public double Price { get; set; }
            private Guid _id;

            public void SetId(Guid id)
            {
                _id = id;
            }

            public Guid GetId()
            {
                return _id;
            }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Name).NotEmpty();
                RuleFor(p => p.Price).NotEmpty().GreaterThanOrEqualTo(0);
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
                var existingPaymentMethod = await _context.PaymentMethods.FindAsync(request.GetId());

                if (existingPaymentMethod == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono metody płatności dla podanego identyfikatora"});
                }

                existingPaymentMethod.Name = request.Name;
                existingPaymentMethod.Price = request.Price;

                _context.PaymentMethods.Update(existingPaymentMethod);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}