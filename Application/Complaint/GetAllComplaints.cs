using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Complaint.Params;
using Application.Complaint.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Complaint
{
    public class GetAllComplaints
    {
        public class Query : IRequest<PagedList<ComplaintResource>>
        {
            public ComplaintParams QueryParams { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedList<ComplaintResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            
            public async Task<PagedList<ComplaintResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var complaints = _context.Complaints
                                                    .Include(p => p.Order)
                                                    .ProjectTo<ComplaintResource>(_mapper.ConfigurationProvider)
                                                    .AsQueryable();

                complaints = FilterByOrderId(complaints, request.QueryParams);


                var complaintsList = await PagedList<ComplaintResource>.ToPagedListAsync(complaints,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);

                return complaintsList;
            }

            private static IQueryable<ComplaintResource> FilterByOrderId(IQueryable<ComplaintResource> complaints, ComplaintParams requestQueryParams)
            {
                if (requestQueryParams.OrderId != Guid.Empty)
                {
                    complaints = complaints.Where(p =>
                        p.Order.Id.ToString().Equals(requestQueryParams.OrderId.ToString()));
                }

                return complaints;
            }
        }
    }
}