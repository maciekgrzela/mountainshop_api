using System;
using System.Collections.Generic;
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
    public class AssignProductToCategories
    {
        public class Command : IRequest
        {
            public Guid ProductId { get; set; }
            public List<Guid> CategoriesIds { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.CategoriesIds).NotNull();
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
                var product = await _context.Products.FindAsync(request.ProductId);

                if (product == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }
                
                var allCategories = await _context.Categories.Select(p => p.Id).ToListAsync(cancellationToken: cancellationToken);
                var existingCategoriesCount = allCategories.Intersect(request.CategoriesIds).Count();

                if (existingCategoriesCount != request.CategoriesIds.Count)
                {
                    throw new RestException(HttpStatusCode.BadRequest,
                        new {info = "Jeden z podanych identyfikatorów kategorii jest niepoprawny"});
                }

                var categories = await _context.Categories.Where(p => request.CategoriesIds.Contains(p.Id)).ToListAsync(cancellationToken: cancellationToken);

                var productsCategories = new List<Domain.Models.ProductsCategories>();
                
                foreach (var category in categories)
                {
                    productsCategories.Add(new Domain.Models.ProductsCategories
                    {
                        Id = Guid.NewGuid(),
                        Product = product,
                        Category = category
                    });
                }

                await _context.ProductsCategories.AddRangeAsync(productsCategories, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}