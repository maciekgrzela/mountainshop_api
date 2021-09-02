using System;
using System.Data;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
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
            public bool ExternalApi { get; set; }
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
                var existingPaymentMethod = await _context.PaymentMethods.FindAsync(request.GetId());

                if (existingPaymentMethod == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono metody płatności dla podanego identyfikatora"});
                }

                existingPaymentMethod.Name = request.Name;
                existingPaymentMethod.Price = request.Price;
                existingPaymentMethod.ExternalApi = request.ExternalApi;

                _context.PaymentMethods.Update(existingPaymentMethod);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}