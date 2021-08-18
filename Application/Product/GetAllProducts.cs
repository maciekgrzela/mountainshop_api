using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Product.Params;
using Application.Product.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Product
{
    public class GetAllProducts
    {
        public class Query : IRequest<PagedList<ProductResource>>
        {
            public ProductParams QueryParams { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedList<ProductResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PagedList<ProductResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var products = _context.Products
                    .Include(p => p.Producer)
                    .Include(p => p.Category)
                    .ProjectTo<ProductResource>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                products = FilterByName(products, request.QueryParams);
                products = FilterByDescription(products, request.QueryParams);
                products = FilterByAmountInStorage(products, request.QueryParams);
                products = FilterByGrossPrice(products, request.QueryParams);
                products = FilterByProducerId(products, request.QueryParams);
                products = FilterByCategoryId(products, request.QueryParams);
                products = SortByGrossPrice(products, request.QueryParams);

                var productsList = await PagedList<ProductResource>.ToPagedListAsync(products, request.QueryParams.PageNumber,
                    request.QueryParams.PageSize);
                
                return productsList;
            }

            private IQueryable<ProductResource> FilterByCategoryId(IQueryable<ProductResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.CategoryId != Guid.Empty)
                {
                    products = products.Where(p => p.Category.Id.ToString().Equals(requestQueryParams.CategoryId.ToString()));
                }

                return products;
            }

            private IQueryable<ProductResource> SortByGrossPrice(IQueryable<ProductResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.GrossPriceAsc != null)
                {
                    products = products.OrderBy(p => p.GrossPrice);
                }
                
                if (requestQueryParams.GrossPriceDesc != null)
                {
                    products = products.OrderByDescending(p => p.GrossPrice);
                }

                return products;
            }

            private IQueryable<ProductResource> FilterByProducerId(IQueryable<ProductResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.ProducerId != Guid.Empty)
                {
                    products = products.Where(p => p.Producer.Id.ToString().Equals(requestQueryParams.ProducerId.ToString()));
                }

                return products;
            }

            private IQueryable<ProductResource> FilterByGrossPrice(IQueryable<ProductResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.GrossPriceMinFilter != null)
                {
                    products = products.Where(p => p.GrossPrice >= requestQueryParams.GrossPriceMinFilter);
                }
                
                if (requestQueryParams.GrossPriceMaxFilter != null)
                {
                    products = products.Where(p => p.GrossPrice <= requestQueryParams.GrossPriceMaxFilter);
                }

                return products;
            }

            private IQueryable<ProductResource> FilterByAmountInStorage(IQueryable<ProductResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.AmountInStorageConstraint != null)
                {
                    products = products.Where(p => p.AmountInStorage <= requestQueryParams.AmountInStorageConstraint);
                }

                return products;
            }

            private IQueryable<ProductResource> FilterByDescription(IQueryable<ProductResource> products, ProductParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.DescriptionFilter))
                {
                    products = products.Where(p => p.Description.Contains(requestQueryParams.DescriptionFilter));
                }

                return products;
            }

            private IQueryable<ProductResource> FilterByName(IQueryable<ProductResource> products, ProductParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.NameFilter))
                {
                    products = products.Where(p => p.Name.Contains(requestQueryParams.NameFilter));
                }

                return products;
            }
        }
    }
}