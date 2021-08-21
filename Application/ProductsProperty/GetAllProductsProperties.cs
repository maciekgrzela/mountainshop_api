using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DeliveryMethod.Resources;
using Application.ProductsProperty.Params;
using Application.ProductsProperty.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.ProductsProperty
{
    public class GetAllProductsProperties
    {
        public class Query : IRequest<PagedList<ProductsPropertyResource>>
        {
            public ProductsPropertyParams QueryParams { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedList<ProductsPropertyResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PagedList<ProductsPropertyResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var productsProperties = _context.ProductsProperties
                    .ProjectTo<ProductsPropertyResource>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                productsProperties = FilterByName(productsProperties, request.QueryParams);
                productsProperties = SortByName(productsProperties, request.QueryParams);

                var productsPropertiesList = await PagedList<ProductsPropertyResource>.ToPagedListAsync(productsProperties,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);
                
                return productsPropertiesList;
            }

            private IQueryable<ProductsPropertyResource> SortByName(IQueryable<ProductsPropertyResource> productsProperties, ProductsPropertyParams requestQueryParams)
            {
                if (requestQueryParams.NameAsc != null)
                {
                    productsProperties = productsProperties.OrderBy(p => p.Name);
                }
                
                if (requestQueryParams.NameAsc != null)
                {
                    productsProperties = productsProperties.OrderByDescending(p => p.Name);
                }

                return productsProperties;
            }

            private IQueryable<ProductsPropertyResource> FilterByName(IQueryable<ProductsPropertyResource> productsProperties, ProductsPropertyParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.NameFilter))
                {
                    productsProperties = productsProperties.Where(p => p.Name.Contains(requestQueryParams.NameFilter));
                }

                return productsProperties;
            }
        }
    }
}