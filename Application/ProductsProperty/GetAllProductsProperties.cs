using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DeliveryMethod.Resources;
using Application.ProductsProperty.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.ProductsProperty
{
    public class GetAllProductsProperties
    {
        public class Query : IRequest<List<ProductsPropertyResource>> { }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator() { }
        }
        
        public class Handler : IRequestHandler<Query, List<ProductsPropertyResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<List<ProductsPropertyResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var productsProperties = await _context.ProductsProperties.ToListAsync(cancellationToken: cancellationToken);
                var productsPropertiesResource =
                    _mapper.Map<List<Domain.Models.ProductsProperty>, List<ProductsPropertyResource>>(productsProperties);

                return productsPropertiesResource;
            }
        }
    }
}