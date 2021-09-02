using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using Application.Producer.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Producer
{
    public class GetProducer
    {
        public class Query : IRequest<ProducerResource>
        {
            public Guid Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
            }
        }
        
        public class Handler : IRequestHandler<Query, ProducerResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<ProducerResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var existingProducer = await _context.Producers.FindAsync(request.Id);

                if (existingProducer == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono metody płatności dla podanego identyfikatora"});
                }

                var producerResource =
                    _mapper.Map<Domain.Models.Producer, ProducerResource>(existingProducer);

                return producerResource;
            }
        }
    }
}