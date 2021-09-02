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
                    .Include(p => p.DeliveryMethods)
                    .ProjectTo<PaymentMethodResource>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                paymentMethods = FilterByPrice(paymentMethods, request.QueryParams.PriceFilter);
                paymentMethods = FilterByName(paymentMethods, request.QueryParams.NameFilter);
                paymentMethods = SortByPrice(paymentMethods, request.QueryParams.PriceSort);
                paymentMethods = SortByName(paymentMethods, request.QueryParams.NameSort);

                var paymentMethodsList = await PagedList<PaymentMethodResource>.ToPagedListAsync(paymentMethods,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);

                return paymentMethodsList;
            }

            private IQueryable<PaymentMethodResource> SortByName(IQueryable<PaymentMethodResource> paymentMethods, bool? filter)
            {
                if (filter != null)
                {
                    paymentMethods = filter.Value
                        ? paymentMethods.OrderBy(p => p.Name)
                        : paymentMethods.OrderByDescending(p => p.Name);
                }

                return paymentMethods;
            }

            private IQueryable<PaymentMethodResource> SortByPrice(IQueryable<PaymentMethodResource> paymentMethods, bool? filter)
            {
                if (filter != null)
                {
                    paymentMethods = filter.Value
                        ? paymentMethods.OrderBy(p => p.Price)
                        : paymentMethods.OrderByDescending(p => p.Price);
                }

                return paymentMethods;
            }

            private IQueryable<PaymentMethodResource> FilterByName(IQueryable<PaymentMethodResource> paymentMethods, string filter)
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    paymentMethods = paymentMethods.Where(p => p.Name.Contains(filter));
                }

                return paymentMethods;
            }

            private IQueryable<PaymentMethodResource> FilterByPrice(IQueryable<PaymentMethodResource> paymentMethods, double? filter)
            {
                if (filter != null)
                {
                    paymentMethods = paymentMethods.Where(p => p.Price.CompareTo(filter) == 0);
                }

                return paymentMethods;
            }
        }
    }
}