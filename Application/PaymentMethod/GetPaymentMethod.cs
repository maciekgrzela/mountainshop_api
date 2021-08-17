using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.PaymentMethod.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.PaymentMethod
{
    public class GetPaymentMethod
    {
        public class Query : IRequest<PaymentMethodResource>
        {
            public Guid Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id).NotEmpty();
            }
        }
        
        public class Handler : IRequestHandler<Query, PaymentMethodResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PaymentMethodResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var existingPaymentMethod = await _context.PaymentMethods.FindAsync(request.Id);

                if (existingPaymentMethod == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono metody płatności dla podanego identyfikatora"});
                }

                var paymentMethodResource =
                    _mapper.Map<Domain.Models.PaymentMethod, PaymentMethodResource>(existingPaymentMethod);

                return paymentMethodResource;
            }
        }
    }
}