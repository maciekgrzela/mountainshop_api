using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.ProductsCategories
{
    public class RemoveProductsCategories
    {
        public class Command : IRequest
        {
            public Guid ProductId { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.ProductId).NotEmpty();
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
                var existingProductsCategories =
                    await _context.ProductsCategories.Where(p => p.ProductId == request.ProductId).ToListAsync(cancellationToken: cancellationToken);

                if (existingProductsCategories == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {data = "Nie znaleziono produktu dla podanego identyfikatora"});
                }

                _context.ProductsCategories.RemoveRange(existingProductsCategories);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}