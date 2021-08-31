using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.PaymentMethod
{
    public class CreatePaymentMethod
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public double Price { get; set; }
            public bool ExternalApi { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Name)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Nazwa nie może być puste")
                    .MinimumLength(10).WithMessage("Pole Nazwa musi posiadać co najmniej 10 znaków");
                RuleFor(p => p.Price)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Cena nie może być puste")
                    .GreaterThanOrEqualTo(0).WithMessage("Wartość pola Cena musi być większa lub równa zero");
                RuleFor(p => p.ExternalApi)
                    .NotEmpty().WithMessage("Pole Obsługi Zewnętrznego Dostawcy nie może być puste");
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
                var paymentMethod = new Domain.Models.PaymentMethod
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Price = request.Price,
                    ExternalApi = request.ExternalApi,
                    Orders = new List<Domain.Models.Order>(),
                    DeliveryMethods = new List<Domain.Models.DeliveryMethod>()
                };

                await _context.PaymentMethods.AddAsync(paymentMethod, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}