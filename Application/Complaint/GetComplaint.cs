using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Complaint.Resources;
using Application.Errors;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Complaint
{
    public class GetComplaint
    {
        public class Query : IRequest<ComplaintResource>
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

        public class Handler : IRequestHandler<Query, ComplaintResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            
            public async Task<ComplaintResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var complaint = await _context.Complaints.FindAsync(request.Id);

                if (complaint == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono reklamacji dla podanego identyfikatora"});
                }

                var complaintResource = _mapper.Map<Domain.Models.Complaint, ComplaintResource>(complaint);

                return complaintResource;
            }
        }
    }
}