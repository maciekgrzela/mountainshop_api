using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Producer.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Producer
{
    public class GetAllProducers
    {
        public class Query : IRequest<List<ProducerResource>> { }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator() { }
        }
        
        public class Handler : IRequestHandler<Query, List<ProducerResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<List<ProducerResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var producers = await _context.Producers.ToListAsync(cancellationToken: cancellationToken);
                var producerResources =
                    _mapper.Map<List<Domain.Models.Producer>, List<ProducerResource>>(producers);

                return producerResources;
            }
        }
    }
}