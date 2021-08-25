using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.DeliveryMethod
{
    public class CreateDeliveryMethod
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public double Price { get; set; }
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
                var deliveryMethod = new Domain.Models.DeliveryMethod
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Price = request.Price,
                    Orders = new List<Domain.Models.Order>(),
                    PaymentMethods = new List<Domain.Models.PaymentMethod>(),
                };

                await _context.DeliveryMethods.AddAsync(deliveryMethod, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}