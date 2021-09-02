using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.PaymentMethod
{
    public class RemovePaymentFromDelivery
    {
        public class Command : IRequest
        {
            public Guid DeliveryId { get; set; }
            public Guid PaymentId { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.DeliveryId)
                    .NotEmpty().WithMessage("Pole Dostawy nie może być puste");
                RuleFor(p => p.PaymentId)
                    .NotEmpty().WithMessage("Pole Płatności nie może być puste");
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
                var existingDelivery = await _context.DeliveryMethods.FindAsync(request.DeliveryId);
                var existingPayment = await _context.PaymentMethods.Include(p => p.DeliveryMethods).FirstOrDefaultAsync(p => p.Id == request.PaymentId);

                if (existingDelivery == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono metody dostawy dla podanego identyfikatora"});
                }
                
                if (existingPayment == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono metody płatności dla podanego identyfikatora"});
                }

                var deliveryIncluded = existingPayment.DeliveryMethods;
                deliveryIncluded.Remove(existingDelivery);

                existingPayment.DeliveryMethods = deliveryIncluded;

                _context.PaymentMethods.Update(existingPayment);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}