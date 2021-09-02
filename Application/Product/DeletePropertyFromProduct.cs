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

namespace Application.Product
{
    public class DeletePropertyFromProduct
    {
        public class Command : IRequest
        {
            public Guid ProductId { get; set; }
            public Guid PropertyId { get; set; }
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
                var existingProperty = await _context.ProductsPropertyValues.FirstOrDefaultAsync(p =>
                    p.ProductId == request.ProductId && p.ProductsPropertyId == request.PropertyId, cancellationToken: cancellationToken);

                if (existingProperty == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono właściwości produktu dla podanego identyfikatora produktu"});
                }

                _context.ProductsPropertyValues.Remove(existingProperty);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}