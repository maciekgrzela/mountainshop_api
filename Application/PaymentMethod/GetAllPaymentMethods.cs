using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.PaymentMethod.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.PaymentMethod
{
    public class GetAllPaymentMethods
    {
        public class Query : IRequest<List<PaymentMethodResource>> { }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator() { }
        }
        
        public class Handler : IRequestHandler<Query, List<PaymentMethodResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<List<PaymentMethodResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var paymentMethods = await _context.PaymentMethods.ToListAsync(cancellationToken: cancellationToken);
                var paymentMethodResource =
                    _mapper.Map<List<Domain.Models.PaymentMethod>, List<PaymentMethodResource>>(paymentMethods);

                return paymentMethodResource;
            }
        }
    }
}