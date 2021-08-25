using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Order
{
    public class CreateOrder
    {
        public class Command : IRequest
        {
            public string UserId { get; set; }
            public string AddressLineOne { get; set; }
            public string PostalCode { get; set; }
            public string Place { get; set; }
            public string Country { get; set; }
            public string PhoneNumber { get; set; }
            public string CompanyName { get; set; }
            public string CompanyNip { get; set; }
            public string CompanyAddressLineOne { get; set; }
            public string CompanyPostalCode { get; set; }
            public string CompanyPlace { get; set; }
            public string CompanyCountry { get; set; }
            public string CompanyPhoneNumber { get; set; }
            public List<SaveOrderedProduct> OrderedProducts { get; set; } 
            public Guid PaymentMethodId { get; set; }
            public Guid DeliveryMethodId { get; set; }
        }
        
        public class SaveOrderedProduct
        {
            public Guid ProductId { get; set; }
            public double Amount { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.UserId).NotEmpty();
                RuleFor(p => p.AddressLineOne).NotEmpty();
                RuleFor(p => p.PostalCode).NotEmpty();
                RuleFor(p => p.Place).NotEmpty();
                RuleFor(p => p.Country).NotEmpty();
                RuleFor(p => p.PhoneNumber).NotEmpty();
                RuleFor(p => p.PaymentMethodId).NotEmpty();
                RuleFor(p => p.DeliveryMethodId).NotEmpty();
                RuleFor(p => p.OrderedProducts).NotEmpty();
            }
        }
        
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IUnitOfWork _unitOfWork;
            private readonly UserManager<Domain.Models.User> _userManager;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IUnitOfWork unitOfWork, UserManager<Domain.Models.User> userManager, IMapper mapper)
            {
                _context = context;
                _unitOfWork = unitOfWork;
                _userManager = userManager;
                _mapper = mapper;
            }
            
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingUser = await _userManager.FindByIdAsync(request.UserId);
                var existingPayment = await _context.PaymentMethods.FindAsync(request.PaymentMethodId);
                var existingDelivery = await _context.DeliveryMethods.FindAsync(request.DeliveryMethodId);

                if (existingUser == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono użytkownika dla podanego identyfikatora"});
                }
                
                if (existingPayment == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono metody płatności dla podanego identyfikatora"});
                }
                
                if (existingDelivery == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono metody dostawy dla podanego identyfikatora"});
                }

                var productsIds = await _context.Products.Select(p => p.Id).ToListAsync(cancellationToken: cancellationToken);
                var orderedIds = request.OrderedProducts.Select(p => p.ProductId).ToList();

                if (productsIds.Intersect(orderedIds).Count() != orderedIds.Count())
                {
                    throw new RestException(HttpStatusCode.BadRequest,
                        new {info = "Identyfikator jednego z podanych produktów jest nieprawidłowy"});
                }

                var orderDetails = new OrderDetails
                {
                    Id = Guid.NewGuid(),
                    AddressLineOne = request.AddressLineOne,
                    CompanyAddressLineOne = request.CompanyAddressLineOne,
                    CompanyCountry = request.CompanyCountry,
                    CompanyName = request.CompanyName,
                    CompanyNip = request.CompanyNip,
                    CompanyPhoneNumber = request.CompanyPhoneNumber,
                    CompanyPlace = request.CompanyPlace,
                    CompanyPostalCode = request.CompanyPostalCode,
                    Country = request.Country,
                    Place = request.Place,
                    PhoneNumber = request.PhoneNumber,
                    PostalCode = request.PostalCode,
                    User = existingUser
                };

                var orderedProducts = request.OrderedProducts.Select(product => new OrderedProduct {Id = Guid.NewGuid(), Amount = product.Amount, ProductId = product.ProductId}).ToList();

                var order = new Domain.Models.Order
                {
                    Id = Guid.NewGuid(),
                    OrderDetails = orderDetails,
                    PaymentMethod = existingPayment,
                    DeliveryMethod = existingDelivery,
                    OrderedProducts = orderedProducts,
                    Status = OrderStatus.Created,
                    WarrantyIsInForce = DateTime.Now.AddDays(30),
                    Created = DateTime.Now,
                };

                await _context.Orders.AddAsync(order, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}