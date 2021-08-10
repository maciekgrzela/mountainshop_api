using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Complaint.Resources;
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Compliant
{
    public class GetAllCompliants
    {
        public class Query : IRequest<List<ComplaintResource>> { }

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
                var complaints = await _context.Complaints.ToListAsync(cancellationToken);
                var complaintsResource = _mapper.Map<List<Domain.Models.Complaint>, List<ComplaintResource>>(complaints);

                return complaintsResource;
            }
        }
    }
}