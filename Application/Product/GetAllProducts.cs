using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Product.Resources;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Product
{
    public class GetAllProducts
    {
        public class Query : IRequest<List<ProductResource>> { }

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
                var products = await _context.Products.Include(p => p.Producer).ToListAsync(cancellationToken: cancellationToken);
                var productsResource = _mapper.Map<List<Domain.Models.Product>, List<ProductResource>>(products);

                return productsResource;
            }
        }
    }
}