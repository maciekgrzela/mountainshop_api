using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Complaint.Resources;
using Application.Errors;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Complaint
{
    public class GetComplaintsForOrder
    {
        public class Query : IRequest<List<ComplaintResource>>
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

        public class Handler : IRequestHandler<Query, List<ComplaintResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<ComplaintResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var order = await _context.Orders.FindAsync(request.Id);

                if (order == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono zamówienia dla podanego identyfikatora"});
                }
                
                var complaints = await _context.Complaints.Where(p => p.OrderId == request.Id).ToListAsync(cancellationToken: cancellationToken);
                var complaintsResource = _mapper.Map<List<Domain.Models.Complaint>, List<ComplaintResource>>(complaints);

                return complaintsResource;
            }
        }
    }
}