using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
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
            
            public Guid PaymentMethodId { get; set; }
            public Guid DeliveryMethodId { get; set; }
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
                throw new System.NotImplementedException();
            }
        }
    }
}