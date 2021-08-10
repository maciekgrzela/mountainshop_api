using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Context;

namespace Application.Category
{
    public class GetAllCategories
    {
        public class Query : IRequest<List<CategoryResource>> { }
        
        public class QueryHandler : IRequestHandler<Query, List<CategoryResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<GetAllCategories> _logger;

            public QueryHandler(DataContext context, IMapper mapper, ILogger<GetAllCategories> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }
            
            public async Task<List<CategoryResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var categories = await _context.Categories.ToListAsync(cancellationToken: cancellationToken);
                var categoriesResource = _mapper.Map<List<Domain.Models.Category>, List<CategoryResource>>(categories);
                return categoriesResource;
            }
        }
    }
}