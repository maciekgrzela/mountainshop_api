using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            public PagingParams QueryParams { get; set; }
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
                var categories = _context.Categories.ProjectTo<CategoryResource>(_mapper.ConfigurationProvider).AsQueryable();
                var categoriesList = await PagedList<CategoryResource>.ToPagedListAsync(categories,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);
                return categoriesList;
            }
        }
    }
}