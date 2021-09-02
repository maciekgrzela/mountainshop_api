using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Contact.Params;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Contact
{
    public class GetAllContactRequests
    {
        public class Query : IRequest<PagedList<ContactRequest>>
        {
            public ContactRequestParams QueryParams { get; set; }
        }
        
        public class Handler : IRequestHandler<Query, PagedList<ContactRequest>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            
            public async Task<PagedList<ContactRequest>> Handle(Query request, CancellationToken cancellationToken)
            {
                var contactRequests = _context.ContactRequests
                    .OrderByDescending(p => p.Created)
                    .AsQueryable();

                contactRequests = FilterByFirstName(contactRequests, request.QueryParams);
                contactRequests = SortByFirstName(contactRequests, request.QueryParams);
                contactRequests = FilterByLastName(contactRequests, request.QueryParams);
                contactRequests = SortByLastName(contactRequests, request.QueryParams);
                contactRequests = FilterByEmail(contactRequests, request.QueryParams);
                contactRequests = SortByEmail(contactRequests, request.QueryParams);
                contactRequests = FilterByContent(contactRequests, request.QueryParams);
                contactRequests = SortByContent(contactRequests, request.QueryParams);
                
                
                return await PagedList<ContactRequest>.ToPagedListAsync(contactRequests, request.QueryParams.PageNumber, request.QueryParams.PageSize);
            }

            private IQueryable<ContactRequest> SortByContent(IQueryable<ContactRequest> contactRequests, ContactRequestParams requestQueryParams)
            {
                if (requestQueryParams.ContentSort != null)
                {
                    contactRequests = requestQueryParams.ContentSort.Value ? contactRequests.OrderBy(p => p.Content) : contactRequests.OrderByDescending(p => p.Content);
                }

                return contactRequests;
            }

            private IQueryable<ContactRequest> FilterByContent(IQueryable<ContactRequest> contactRequests, ContactRequestParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.ContentFilter))
                {
                    contactRequests =
                        contactRequests.Where(p => p.Content.Contains(requestQueryParams.ContentFilter));
                }

                return contactRequests;
            }

            private IQueryable<ContactRequest> SortByEmail(IQueryable<ContactRequest> contactRequests, ContactRequestParams requestQueryParams)
            {
                if (requestQueryParams.EmailSort != null)
                {
                    contactRequests = requestQueryParams.EmailSort.Value ? contactRequests.OrderBy(p => p.Email) : contactRequests.OrderByDescending(p => p.Email);
                }

                return contactRequests;
            }

            private IQueryable<ContactRequest> FilterByEmail(IQueryable<ContactRequest> contactRequests, ContactRequestParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.EmailFilter))
                {
                    contactRequests =
                        contactRequests.Where(p => p.Email.Contains(requestQueryParams.EmailFilter));
                }

                return contactRequests;
            }

            private IQueryable<ContactRequest> SortByLastName(IQueryable<ContactRequest> contactRequests, ContactRequestParams requestQueryParams)
            {
                if (requestQueryParams.LastNameSort != null)
                {
                    contactRequests = requestQueryParams.LastNameSort.Value ? contactRequests.OrderBy(p => p.LastName) : contactRequests.OrderByDescending(p => p.LastName);
                }

                return contactRequests;
            }

            private IQueryable<ContactRequest> FilterByLastName(IQueryable<ContactRequest> contactRequests, ContactRequestParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.LastNameFilter))
                {
                    contactRequests =
                        contactRequests.Where(p => p.LastName.Contains(requestQueryParams.LastNameFilter));
                }

                return contactRequests;
            }

            private IQueryable<ContactRequest> SortByFirstName(IQueryable<ContactRequest> contactRequests, ContactRequestParams requestQueryParams)
            {
                if (requestQueryParams.FirstNameSort != null)
                {
                    contactRequests = requestQueryParams.FirstNameSort.Value ? contactRequests.OrderBy(p => p.FirstName) : contactRequests.OrderByDescending(p => p.FirstName);
                }

                return contactRequests;
            }

            private IQueryable<ContactRequest> FilterByFirstName(IQueryable<ContactRequest> contactRequests, ContactRequestParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.FirstNameFilter))
                {
                    contactRequests =
                        contactRequests.Where(p => p.FirstName.Contains(requestQueryParams.FirstNameFilter));
                }

                return contactRequests;
            }
        }
    }
}