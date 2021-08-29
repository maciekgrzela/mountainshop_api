using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Contact
{
    public class GetAllContactRequests
    {
        public class Query : IRequest<List<ContactRequest>> { }
        
        public class Handler : IRequestHandler<Query, List<ContactRequest>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            
            public async Task<List<ContactRequest>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.ContactRequests.ToListAsync(cancellationToken: cancellationToken);
            }
        }
    }
}