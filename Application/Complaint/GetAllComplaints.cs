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

                complaints = FilterByOrder(complaints, request.QueryParams.OrderFilter);
                complaints = SortByOrder(complaints, request.QueryParams.OrderSort);
                complaints = FilterByNumber(complaints, request.QueryParams.NumberFilter);
                complaints = SortByNumber(complaints, request.QueryParams.NumberSort);
                complaints = FilterByAbbreviation(complaints, request.QueryParams.AbbreviationFilter);
                complaints = SortByAbbreviation(complaints, request.QueryParams.AbbreviationSort);
                complaints = FilterByDescription(complaints, request.QueryParams.DescriptionFilter);
                complaints = SortByDescription(complaints, request.QueryParams.DescriptionSort);


                var complaintsList = await PagedList<ComplaintResource>.ToPagedListAsync(complaints,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);

                return complaintsList;
            }

            private IQueryable<ComplaintResource> SortByDescription(IQueryable<ComplaintResource> complaints, bool? queryParamsDescriptionSort)
            {
                if (queryParamsDescriptionSort != null)
                {
                    complaints = queryParamsDescriptionSort.Value
                        ? complaints.OrderBy(p => p.Description)
                        : complaints.OrderByDescending(p => p.Description);
                }

                return complaints;
            }

            private IQueryable<ComplaintResource> FilterByDescription(IQueryable<ComplaintResource> complaints, string queryParamsDescriptionFilter)
            {
                if (!string.IsNullOrWhiteSpace(queryParamsDescriptionFilter))
                {
                    complaints = complaints.Where(p => p.Description.Contains(queryParamsDescriptionFilter));
                }

                return complaints;
            }

            private IQueryable<ComplaintResource> SortByAbbreviation(IQueryable<ComplaintResource> complaints, bool? queryParamsAbbreviationSort)
            {
                if (queryParamsAbbreviationSort != null)
                {
                    complaints = queryParamsAbbreviationSort.Value
                        ? complaints.OrderBy(p => p.Abbreviation)
                        : complaints.OrderByDescending(p => p.Abbreviation);
                }

                return complaints;
            }

            private IQueryable<ComplaintResource> FilterByAbbreviation(IQueryable<ComplaintResource> complaints, string queryParamsAbbreviationFilter)
            {
                if (!string.IsNullOrWhiteSpace(queryParamsAbbreviationFilter))
                {
                    complaints = complaints.Where(p => p.Abbreviation.Contains(queryParamsAbbreviationFilter));
                }

                return complaints;
            }

            private IQueryable<ComplaintResource> SortByNumber(IQueryable<ComplaintResource> complaints, bool? queryParamsNumberSort)
            {
                if (queryParamsNumberSort != null)
                {
                    complaints = queryParamsNumberSort.Value
                        ? complaints.OrderBy(p => p.Number)
                        : complaints.OrderByDescending(p => p.Number);
                }

                return complaints;
            }

            private IQueryable<ComplaintResource> FilterByNumber(IQueryable<ComplaintResource> complaints, int? queryParamsNumberFilter)
            {
                if (queryParamsNumberFilter != null)
                {
                    complaints = complaints.Where(p => p.Number == queryParamsNumberFilter);
                }

                return complaints;
            }

            private IQueryable<ComplaintResource> SortByOrder(IQueryable<ComplaintResource> complaints, bool? queryParamsOrderSort)
            {
                if (queryParamsOrderSort != null)
                {
                    complaints = queryParamsOrderSort.Value
                        ? complaints.OrderBy(p => p.Order.Id)
                        : complaints.OrderByDescending(p => p.Order.Id);
                }

                return complaints;
            }

            private static IQueryable<ComplaintResource> FilterByOrder(IQueryable<ComplaintResource> complaints, Guid? filter)
            {
                if (filter != null)
                {
                    complaints = complaints.Where(p =>
                        p.Order.Id.ToString().Equals(filter.ToString()));
                }

                return complaints;
            }
        }
    }
}