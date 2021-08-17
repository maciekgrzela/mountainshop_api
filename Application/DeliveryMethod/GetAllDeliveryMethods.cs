using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DeliveryMethod.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.DeliveryMethod
{
    public class GetAllDeliveryMethods
    {
        public class Query : IRequest<List<DeliveryMethodResource>> { }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator() { }
        }
        
        public class Handler : IRequestHandler<Query, List<DeliveryMethodResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<List<DeliveryMethodResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var deliveryMethods = await _context.DeliveryMethods.ToListAsync(cancellationToken: cancellationToken);
                var deliveryMethodsResource =
                    _mapper.Map<List<Domain.Models.DeliveryMethod>, List<DeliveryMethodResource>>(deliveryMethods);

                return deliveryMethodsResource;
            }
        }
    }
}