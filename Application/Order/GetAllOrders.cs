using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Category;
using Application.Order.Resources;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Order
{
    public class GetAllOrders
    {
        public class Query : IRequest<List<OrderResource>> { }
        
        public class QueryHandler : IRequestHandler<Query, List<OrderResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<List<OrderResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var orders = await _context.Orders
                    .Include(p => p.DeliveryMethod)
                    .Include(p => p.PaymentMethod)
                    .Include(p => p.OrderDetails)
                    .Include(p => p.OrderedProducts)
                    .ToListAsync(cancellationToken: cancellationToken);
                var ordersResource = _mapper.Map<List<Domain.Models.Order>, List<OrderResource>>(orders);
                return ordersResource;
            }
        }
    }
}