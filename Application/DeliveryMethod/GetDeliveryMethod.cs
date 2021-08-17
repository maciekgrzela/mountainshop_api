using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.DeliveryMethod.Resources;
using Application.Errors;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.DeliveryMethod
{
    public class GetDeliveryMethod
    {
        public class Query : IRequest<DeliveryMethodResource>
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
        
        public class Handler : IRequestHandler<Query, DeliveryMethodResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<DeliveryMethodResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var existingDeliveryMethod = await _context.DeliveryMethods.FindAsync(request.Id);

                if (existingDeliveryMethod == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono metody płatności dla podanego identyfikatora"});
                }

                var deliveryMethodResource =
                    _mapper.Map<Domain.Models.DeliveryMethod, DeliveryMethodResource>(existingDeliveryMethod);

                return deliveryMethodResource;
            }
        }
    }
}