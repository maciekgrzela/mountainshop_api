using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Category.Params;
using Application.Category.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence.Context;

namespace Application.Category
{
    public class GetAllCategories
    {
        public class Query : IRequest<PagedList<CategoryResource>>
        {
            public CategoryParams QueryParams { get; set; }
        }
        
        public class QueryHandler : IRequestHandler<Query, PagedList<CategoryResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PagedList<CategoryResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var categories = _context.Categories
                    .ProjectTo<CategoryResource>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                categories = FilterByName(categories, request.QueryParams.NameFilter);
                categories = SortByName(categories, request.QueryParams.NameSort);
                categories = FilterByDescription(categories, request.QueryParams.DescriptionFilter);
                categories = SortByDescription(categories, request.QueryParams.DescriptionSort);
                
                return await PagedList<CategoryResource>.ToPagedListAsync(categories,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);
            }

            private IQueryable<CategoryResource> SortByDescription(IQueryable<CategoryResource> categories, bool? queryParamsDescriptionSort)
            {
                if (queryParamsDescriptionSort != null)
                {
                    categories = queryParamsDescriptionSort.Value
                        ? categories.OrderBy(p => p.Description)
                        : categories.OrderByDescending(p => p.Description);
                }

                return categories;
            }

            private IQueryable<CategoryResource> FilterByDescription(IQueryable<CategoryResource> categories, string queryParamsDescriptionFilter)
            {
                if (!string.IsNullOrWhiteSpace(queryParamsDescriptionFilter))
                {
                    categories = categories.Where(p => p.Description.Contains(queryParamsDescriptionFilter));
                }

                return categories;
            }

            private IQueryable<CategoryResource> SortByName(IQueryable<CategoryResource> categories, bool? queryParamsNameSort)
            {
                if (queryParamsNameSort != null)
                {
                    categories = queryParamsNameSort.Value
                        ? categories.OrderBy(p => p.Name)
                        : categories.OrderByDescending(p => p.Name);
                }

                return categories;
            }

            private IQueryable<CategoryResource> FilterByName(IQueryable<CategoryResource> categories, string queryParamsNameFilter)
            {
                if (!string.IsNullOrWhiteSpace(queryParamsNameFilter))
                {
                    categories = categories.Where(p => p.Name.Contains(queryParamsNameFilter));
                }

                return categories;
            }
        }
    }
}