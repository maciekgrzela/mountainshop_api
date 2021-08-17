using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.PaymentMethod.Params;
using Application.PaymentMethod.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.PaymentMethod
{
    public class GetAllPaymentMethods
    {
        public class Query : IRequest<PagedList<PaymentMethodResource>>
        {
            public PaymentMethodParams QueryParams { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedList<PaymentMethodResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PagedList<PaymentMethodResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var paymentMethods = _context.PaymentMethods
                    .ProjectTo<PaymentMethodResource>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                paymentMethods = FilterByPrice(paymentMethods, request.QueryParams);
                paymentMethods = FilterByName(paymentMethods, request.QueryParams);
                paymentMethods = SortByPrice(paymentMethods, request.QueryParams);
                paymentMethods = SortByName(paymentMethods, request.QueryParams);

                var paymentMethodsList = await PagedList<PaymentMethodResource>.ToPagedListAsync(paymentMethods,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);

                return paymentMethodsList;
            }

            private IQueryable<PaymentMethodResource> SortByName(IQueryable<PaymentMethodResource> paymentMethods, PaymentMethodParams requestQueryParams)
            {
                if (requestQueryParams.NameAsc != null)
                {
                    paymentMethods = paymentMethods.OrderBy(p => p.Name);
                }

                if (requestQueryParams.NameDesc != null)
                {
                    paymentMethods = paymentMethods.OrderByDescending(p => p.Name);
                }

                return paymentMethods;
            }

            private IQueryable<PaymentMethodResource> SortByPrice(IQueryable<PaymentMethodResource> paymentMethods, PaymentMethodParams requestQueryParams)
            {
                if (requestQueryParams.PriceAsc != null)
                {
                    paymentMethods = paymentMethods.OrderBy(p => p.Price);
                }

                if (requestQueryParams.PriceDesc != null)
                {
                    paymentMethods = paymentMethods.OrderByDescending(p => p.Price);
                }

                return paymentMethods;
            }

            private IQueryable<PaymentMethodResource> FilterByName(IQueryable<PaymentMethodResource> paymentMethods, PaymentMethodParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.NameFilter))
                {
                    paymentMethods = paymentMethods.Where(p => p.Name.Contains(requestQueryParams.NameFilter));
                }

                return paymentMethods;
            }

            private IQueryable<PaymentMethodResource> FilterByPrice(IQueryable<PaymentMethodResource> paymentMethods, PaymentMethodParams requestQueryParams)
            {
                if (requestQueryParams.PriceFilter != null)
                {
                    paymentMethods = paymentMethods.Where(p =>
                        requestQueryParams.PriceFilter != null && p.Price.CompareTo(requestQueryParams.PriceFilter) == 0);
                }

                return paymentMethods;
            }
        }
    }
}