using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Order;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;
using Stripe;
using Stripe.Checkout;

namespace Application.Checkout
{
    public class CreateCheckoutSession
    {
        public class Command : IRequest<string>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly DataContext _context;
            private readonly UserManager<Domain.Models.User> _userManager;
            private readonly IMediator _mediator;
            private IConfiguration Configuration { get; }

            public Handler(DataContext context, UserManager<Domain.Models.User> userManager, IConfiguration configuration, IMediator mediator)
            {
                _context = context;
                _userManager = userManager;
                _mediator = mediator;
                Configuration = configuration;
            }
            
            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                var domain = $"{Configuration.GetSection("ClientAppUrl").Value}/order/";
                var itemOptions = await GenerateSessionLineItemOptionsAsync(request.Id);
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card", "p24"
                    },
                    LineItems = itemOptions,
                    Mode = "payment",
                    SuccessUrl = $"{domain}success?stripe_redirect=true",
                    CancelUrl = $"{domain}canceled?stripe_redirect=true"
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

                return session.Url;
            }

            private async Task<List<SessionLineItemOptions>> GenerateSessionLineItemOptionsAsync(string userId)
            {
                var existingUser = await _userManager.FindByIdAsync(userId);

                if (existingUser == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono użytkownika dla podanego identyfikatora"});
                }
                
                var order = await _context.Orders
                    .Include(p => p.OrderDetails)
                    .Include(p => p.DeliveryMethod)
                    .Include(p => p.PaymentMethod)
                    .Include(p => p.OrderedProducts)
                    .ThenInclude(p => p.Product)
                    .OrderByDescending(p => p.Created)
                    .FirstOrDefaultAsync(p => p.OrderDetails.UserId == userId);

                if (order == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono zamówienia dla podanego identyfikatora"});
                }

                var lineItems = new List<SessionLineItemOptions>();
                var productService = new ProductService();
                var priceService = new PriceService();

                if (order.DeliveryMethod.Price > 0)
                {
                    var deliveryOptions = new ProductCreateOptions
                    {
                        Name = "Koszty dostawy"
                    };
                    var deliveryOption = await productService.CreateAsync(deliveryOptions);

                    var deliveryPriceOptions = new PriceCreateOptions
                    {
                        Product = deliveryOption.Id,
                        UnitAmountDecimal = (decimal?) order.DeliveryMethod.Price * 100,
                        Currency = "pln"
                    };

                    var deliveryPrice = await priceService.CreateAsync(deliveryPriceOptions);
                    
                    lineItems.Add(new SessionLineItemOptions
                    {
                        Price = deliveryPrice.Id,
                        Quantity = 1
                    });
                }
                
                if (order.PaymentMethod.Price > 0)
                {
                    var paymentOptions = new ProductCreateOptions
                    {
                        Name = "Koszty płatności"
                    };
                    var paymentOption = await productService.CreateAsync(paymentOptions);

                    var paymentPriceOptions = new PriceCreateOptions
                    {
                        Product = paymentOption.Id,
                        UnitAmountDecimal = (decimal?) order.DeliveryMethod.Price * 100,
                        Currency = "pln"
                    };

                    var paymentPrice = await priceService.CreateAsync(paymentPriceOptions);
                    
                    lineItems.Add(new SessionLineItemOptions
                    {
                        Price = paymentPrice.Id,
                        Quantity = 1
                    });
                }

                foreach (var orderedProduct in order.OrderedProducts)
                {
                    var productOptions = new ProductCreateOptions
                    {
                        Name = orderedProduct.Product.Name
                    };
                    var product = await productService.CreateAsync(productOptions);

                    var priceOptions = new PriceCreateOptions
                    {
                        Product = product.Id,
                        UnitAmountDecimal = (decimal?) orderedProduct.Product.GrossPrice * 100,
                        Currency = "pln"
                    };

                    var price = await priceService.CreateAsync(priceOptions);
                    
                    lineItems.Add(new SessionLineItemOptions
                    {
                        Price = price.Id,
                        Quantity = (long?) orderedProduct.Amount
                    });
                }
                
                return lineItems;
            }
        }
    }
}