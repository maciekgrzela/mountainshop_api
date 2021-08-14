using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Category
{
    public class GetCategoriesForProduct
    {
        public class Query : IRequest<List<CategoryResource>>
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
        
        public class Handler : IRequestHandler<Query, List<CategoryResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<List<CategoryResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var product = await _context.Products.FindAsync(request.Id);

                if (product == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }

                var productsCategories = await _context.ProductsCategories.Where(p => p.ProductId == request.Id).ToListAsync(cancellationToken: cancellationToken);

                var categories = productsCategories.Select(p => p.Category).ToList();
                var categoriesResource = _mapper.Map<List<Domain.Models.Category>, List<CategoryResource>>(categories);

                return categoriesResource;
            }
        }
    }
}