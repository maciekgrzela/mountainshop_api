using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Checkout.Resources;
using Application.Core;
using Application.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Stripe.Checkout;

namespace Application.Checkout
{
    public class GetCheckoutSession
    {
        public class Query : IRequest<CheckoutSessionResource>
        {
            public string Id { get; set; }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator musi posiadać wartość");
            }
        }
        
        public class Handler : IRequestHandler<Query, CheckoutSessionResource>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            
            
            public async Task<CheckoutSessionResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(request.Id, cancellationToken: cancellationToken);
                var checkoutOrderSessionId = Guid.Parse(session.Metadata["OrderId"]);
                var order = await _context.Orders
                    .Include(p => p.DeliveryMethod)
                    .Include(p => p.PaymentMethod)
                    .Include(p => p.OrderDetails)
                    .Include(p => p.OrderedProducts)
                    .FirstOrDefaultAsync(p => p.Id == checkoutOrderSessionId, cancellationToken: cancellationToken);

                if (order == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono zamówienia dla podanego identyfikatora sesji"});
                }

                return new CheckoutSessionResource
                {
                    OrderVerified = true
                };
            }
        }
    }
}