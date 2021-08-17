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
    public class AssignProductToCategory
    {
        public class Command : IRequest
        {
            public Guid ProductId { get; set; }
            public Guid CategoryId { get; set; }
        }
        
        public class CommandValiadtor : AbstractValidator<Command>
        {
            public CommandValiadtor()
            {
                RuleFor(p => p.CategoryId).NotEmpty();
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
                var existingProductsCategory = await _context.ProductsCategories.Where(p =>
                    p.CategoryId == request.CategoryId && p.ProductId == request.ProductId).ToListAsync(cancellationToken: cancellationToken);

                if (existingProductsCategory != null)
                {
                    throw new RestException(HttpStatusCode.BadRequest,
                        new {info = "Produkt jest już przypisany do podanej kategorii"});
                }
                
                var product = await _context.Products.FindAsync(request.ProductId);

                if (product == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }
                
                var category = await _context.Categories.FindAsync(request.CategoryId);

                if (category == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono kategorii dla podanego identyfikatora"});
                }

                var productsCategories = new Domain.Models.ProductsCategories
                {
                    Id = Guid.NewGuid(),
                    Product = product,
                    Category = category
                };

                await _context.ProductsCategories.AddAsync(productsCategories, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}