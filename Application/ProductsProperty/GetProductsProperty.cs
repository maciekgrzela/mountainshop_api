using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.ProductsProperty.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.ProductsProperty
{
    public class GetProductsProperty
    {
        public class Query : IRequest<ProductsPropertyResource>
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
        
        public class Handler : IRequestHandler<Query, ProductsPropertyResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<ProductsPropertyResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var existingProductsProperty = await _context.ProductsProperties.FindAsync(request.Id);

                if (existingProductsProperty == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono właściwości produktu dla podanego identyfikatora"});
                }

                var productsPropertyResource =
                    _mapper.Map<Domain.Models.ProductsProperty, ProductsPropertyResource>(existingProductsProperty);

                return productsPropertyResource;
            }
        }
    }
}