using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Product.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Product
{
    public class GetProduct
    {
        public class Query : IRequest<ProductResource>
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
        
        public class Handler : IRequestHandler<Query,ProductResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            
            public async Task<ProductResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var existingProduct = await _context.Products.FindAsync(request.Id);

                if (existingProduct == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }

                var productResource = _mapper.Map<Domain.Models.Product, ProductResource>(existingProduct);
                return productResource;
            }
        }
    }
}