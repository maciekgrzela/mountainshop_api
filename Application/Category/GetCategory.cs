using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Category.Resources;
using Application.Errors;
using AutoMapper;
using MediatR;
using Persistence.Context;

namespace Application.Category
{
    public class GetCategory
    {
        public class Query : IRequest<CategoryResource>
        {
            public Guid Id { get; set; }
        }
        
        public class Handler : IRequestHandler<Query, CategoryResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<CategoryResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var category = await _context.Categories.FindAsync(request.Id);

                if (category == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Kategoria o podanym identyfikatorze nie została znaleziona"});
                }

                var categoryResource = _mapper.Map<Domain.Models.Category, CategoryResource>(category);

                return categoryResource;
            }
        }
    }
}