using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Order.Resources;
using AutoMapper;
using MediatR;
using Persistence.Context;

namespace Application.Order
{
    public class GetOrder
    {
        public class Query : IRequest<OrderResource>
        {
            public Guid Id { get; set; }
        }
        
        public class Handler : IRequestHandler<Query, OrderResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<OrderResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var order = await _context.Orders.FindAsync(request.Id);

                if (order == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Kategoria o podanym identyfikatorze nie została znaleziona"});
                }

                var orderResource = _mapper.Map<Domain.Models.Order, OrderResource>(order);

                return orderResource;
            }
        }
    }
}