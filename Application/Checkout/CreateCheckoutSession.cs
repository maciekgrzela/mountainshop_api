using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

namespace Application.Checkout
{
    public class CreateCheckoutSession
    {
        public class Command : IRequest<string> { }

        public class Handler : IRequestHandler<Command, string>
        {
            private IConfiguration Configuration { get; }

            public Handler(IConfiguration configuration)
            {
                Configuration = configuration;
            }
            
            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                var domain = $"{Configuration.GetSection("ClientAppUrl").Value}/payment/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card", "p24"
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Price = "price_1JPQ9AH04NHH9AzUJcsQANJK",
                            Quantity = 1,
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = $"{domain}?success=true",
                    CancelUrl = $"{domain}?canceled=true"
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

                return session.Url;
            }
        }
    }
}