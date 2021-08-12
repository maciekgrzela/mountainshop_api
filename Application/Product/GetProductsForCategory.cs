using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Category;
using Application.Errors;
using Application.Product.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Product
{
    public class GetProductsForCategory
    {
        public class Query : IRequest<List<ProductResource>>
        {
            public Guid Id { get; set; }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id).NotEmpty();
            }
        }
        
        public class Handler : IRequestHandler<Query, List<ProductResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<List<ProductResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var category = await _context.Categories.FindAsync(request.Id);

                if (category == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }

                var products = await _context.ProductsCategories.Where(p => p.CategoryId == request.Id).ToListAsync(cancellationToken: cancellationToken);
                var productsResource = _mapper.Map<List<Domain.Models.ProductsCategories>, List<ProductResource>>(products);

                return productsResource;
            }
        }
    }
}