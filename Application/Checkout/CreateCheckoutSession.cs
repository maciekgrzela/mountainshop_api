﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using Application.Order;
using FluentValidation;
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
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikatora nie może być puste");
            }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly DataContext _context;
            private readonly UserManager<Domain.Models.User> _userManager;
            private IConfiguration Configuration { get; }

            public Handler(DataContext context, UserManager<Domain.Models.User> userManager, IConfiguration configuration)
            {
                _context = context;
                _userManager = userManager;
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
                    LineItems = itemOptions.Item1,
                    Mode = "payment",
                    SuccessUrl = $"{domain}created?stripe_redirect=true&session={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{domain}created?stripe_redirect=false&session={{CHECKOUT_SESSION_ID}}",
                    Metadata = new Dictionary<string, string>
                    {
                        {"OrderId", itemOptions.Item2.ToString()}
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

                return session.Url;
            }

            private async Task<(List<SessionLineItemOptions>, Guid)> GenerateSessionLineItemOptionsAsync(string userId)
            {
                var existingUser = await _userManager.FindByIdAsync(userId);

                if (existingUser == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
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
                    throw new RestException(HandlerResponse.ResourceNotFound,
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
                        UnitAmount = (long?) (Math.Round(order.DeliveryMethod.Price,2) * 100),
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
                        UnitAmount = (long?) (Math.Round(order.DeliveryMethod.Price,2) * 100),
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
                        UnitAmount = (long?) (Math.Round(orderedProduct.Product.GrossPrice, 2) * 100),
                        Currency = "pln"
                    };

                    var price = await priceService.CreateAsync(priceOptions);

                    lineItems.Add(new SessionLineItemOptions
                    {
                        Price = price.Id,
                        Quantity = (long?) orderedProduct.Amount
                    });
                }
                
                return (lineItems, order.Id);
            }
        }
    }
}