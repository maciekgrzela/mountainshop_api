using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain.Models;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.ProductsProperty
{
    public class AddPropertyToProduct
    {
        public class Command : Request, IRequest
        {
            public Guid ProductId { get; set; }
            public Guid PropertyId { get; set; }
        }
        
        public class Request
        {
            public string Value { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.ProductId)
                    .NotEmpty().WithMessage("Pole Produkt nie może być puste");
                RuleFor(p => p.PropertyId)
                    .NotEmpty().WithMessage("Pole Właściwość nie może być puste");
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
                var existingProduct = await _context.Products.FindAsync(request.ProductId);

                if (existingProduct == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }
                
                var existingProperty = await _context.ProductsProperties.FindAsync(request.PropertyId);

                if (existingProperty == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono właściwości dla podanego identyfikatora"});
                }

                var productsPropertyValue = new ProductsPropertyValue
                {
                    Id = Guid.NewGuid(),
                    Product = existingProduct,
                    ProductsProperty = existingProperty,
                    Value = request.Value
                };

                await _context.ProductsPropertyValues.AddAsync(productsPropertyValue, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}