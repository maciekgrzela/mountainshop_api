using System;
using System.Collections.Generic;
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
                    .Include(p => p.Comments)
                    .OrderByDescending(p => p.Created)
                    .ProjectTo<ProductWithCommentsResource>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                products = FilterByName(products, request.QueryParams);
                products = FilterByDescription(products, request.QueryParams);
                products = FilterByGender(products, request.QueryParams);
                products = FilterByAmountInStorage(products, request.QueryParams);
                products = FilterByGrossPrice(products, request.QueryParams);
                products = FilterByProducerIds(products, request.QueryParams);
                products = FilterByCategoryId(products, request.QueryParams);
                products = FilterBySale(products, request.QueryParams);
                products = FilterByDate(products, request.QueryParams);
                products = await FilterByCommentsRating(products, request.QueryParams);
                products = await FilterByCommentsCount(products, request.QueryParams);
                products = SortByGrossPrice(products, request.QueryParams);

                var productsList = await products.ToListAsync(cancellationToken: cancellationToken);
                var resourcesList = _mapper.Map<List<ProductWithCommentsResource>, List<ProductResource>>(productsList);
                
                var productsResources = PagedList<ProductResource>.ToPagedList(resourcesList, request.QueryParams.PageNumber, request.QueryParams.PageSize);
                
                return productsResources;
            }

            private async Task<IQueryable<ProductWithCommentsResource>> FilterByCommentsCount(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.CommentsCountDesc != null)
                {
                    products = products.OrderByDescending(p => p.Comments.Count);
                }

                return products;
            }

            private async Task<IQueryable<ProductWithCommentsResource>> FilterByCommentsRating(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.BestRatingDesc != null)
                {
                    products = products.OrderByDescending(p => p.Comments.Average(c => c.Rate));
                }

                return products;
            }

            private IQueryable<ProductWithCommentsResource> FilterByGender(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.GenderFilter != null && requestQueryParams.GenderFilter.Count > 0)
                {
                    products = products.Where(p => requestQueryParams.GenderFilter.Contains(p.Gender));
                }

                return products;
            }

            private IQueryable<ProductWithCommentsResource> FilterByDate(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.TheNewFilter != null)
                {
                    products = products.Where(p => DateTime.Now.AddDays(-30).CompareTo(p.Created) <= 0);
                }

                return products;
            }

            private IQueryable<ProductWithCommentsResource> FilterBySale(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.SaleFilter != null)
                {
                    products = products.Where(p => p.PercentageSale != null);
                }

                return products;
            }

            private IQueryable<ProductWithCommentsResource> FilterByCategoryId(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.CategoryId != Guid.Empty)
                {
                    products = products.Where(p => p.Category.Id.ToString().Equals(requestQueryParams.CategoryId.ToString()));
                }

                return products;
            }

            private IQueryable<ProductWithCommentsResource> SortByGrossPrice(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
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

            private IQueryable<ProductWithCommentsResource> FilterByProducerIds(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.ProducerIds != null && requestQueryParams.ProducerIds.Count > 0)
                {
                    products = products.Where(p => string.Join(" ", requestQueryParams.ProducerIds).Contains(p.Producer.Id.ToString()));
                }

                return products;
            }

            private IQueryable<ProductWithCommentsResource> FilterByGrossPrice(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
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

            private IQueryable<ProductWithCommentsResource> FilterByAmountInStorage(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (requestQueryParams.AmountInStorageConstraint != null)
                {
                    products = products.Where(p => p.AmountInStorage >= requestQueryParams.AmountInStorageConstraint);
                }

                return products;
            }

            private IQueryable<ProductWithCommentsResource> FilterByDescription(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.DescriptionFilter))
                {
                    products = products.Where(p => p.Description.Contains(requestQueryParams.DescriptionFilter));
                }

                return products;
            }

            private IQueryable<ProductWithCommentsResource> FilterByName(IQueryable<ProductWithCommentsResource> products, ProductParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.NameFilter))
                {
                    products = products.Where(p => p.Name.Contains(requestQueryParams.NameFilter));
                }

                return products;
            }
        }
    }

    internal class RatingsComparer : IComparer<ProductResource>
    {
        private readonly List<Guid> _ratingGuids;

        public RatingsComparer(List<Guid> ratingGuids)
        {
            _ratingGuids = ratingGuids;
        }

        public int Compare(ProductResource? x, ProductResource? y)
        {
            if (x == null || y == null) return 0;
            
            var xPosition = _ratingGuids.IndexOf(x.Id);
            var yPosition = _ratingGuids.IndexOf(y.Id);

            return xPosition - yPosition;

        }
    }
}