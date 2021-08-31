using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Order.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Order
{
    public class GetOrdersForUser
    {
        public class Query : IRequest<List<OrderResource>>
        {
            public string Id { get; set; }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
            }
        }
        
        public class Handler : IRequestHandler<Query, List<OrderResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
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
                    .ThenInclude(p => p.User)
                    .Include(p => p.OrderedProducts)
                    .ThenInclude(p => p.Product)
                    .Where(p => p.OrderDetails.User.Id == request.Id)
                    .ToListAsync(cancellationToken: cancellationToken);
                
                var ordersResource = _mapper.Map<List<Domain.Models.Order>, List<OrderResource>>(orders);
                return ordersResource;
            }
        }
    }
}