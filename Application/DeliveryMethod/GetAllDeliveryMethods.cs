using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DeliveryMethod.Params;
using Application.DeliveryMethod.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.DeliveryMethod
{
    public class GetAllDeliveryMethods
    {
        public class Query : IRequest<PagedList<DeliveryMethodResource>>
        {
            public DeliveryMethodParams QueryParams { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedList<DeliveryMethodResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PagedList<DeliveryMethodResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var deliveryMethods = _context.DeliveryMethods
                    .Include(p => p.PaymentMethods)
                    .ProjectTo<DeliveryMethodResource>(_mapper.ConfigurationProvider)
                    .OrderBy(p => p.Price)
                    .AsQueryable();

                deliveryMethods = FilterByPrice(deliveryMethods, request.QueryParams.PriceFilter);
                deliveryMethods = SortByPrice(deliveryMethods, request.QueryParams.PriceSort);
                deliveryMethods = FilterByName(deliveryMethods, request.QueryParams.NameFilter);
                deliveryMethods = SortByName(deliveryMethods, request.QueryParams.NameSort);

                var deliveryMethodsList = await PagedList<DeliveryMethodResource>.ToPagedListAsync(deliveryMethods,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);

                return deliveryMethodsList;
            }

            private IQueryable<DeliveryMethodResource> SortByName(IQueryable<DeliveryMethodResource> deliveryMethods, bool? queryParamsNameSort)
            {
                if (queryParamsNameSort != null)
                {
                    deliveryMethods = queryParamsNameSort.Value
                        ? deliveryMethods.OrderBy(p => p.Name)
                        : deliveryMethods.OrderByDescending(p => p.Name);
                }

                return deliveryMethods;
            }

            private IQueryable<DeliveryMethodResource> FilterByName(IQueryable<DeliveryMethodResource> deliveryMethods, string queryParamsNameFilter)
            {
                if (queryParamsNameFilter != null)
                {
                    deliveryMethods = deliveryMethods.Where(p => p.Name.Contains(queryParamsNameFilter));
                }

                return deliveryMethods;
            }

            private IQueryable<DeliveryMethodResource> SortByPrice(IQueryable<DeliveryMethodResource> deliveryMethods, bool? filter)
            {
                if (filter != null)
                {
                    deliveryMethods = filter.Value
                        ? deliveryMethods.OrderBy(p => p.Price)
                        : deliveryMethods.OrderByDescending(p => p.Price);
                }

                return deliveryMethods;
            }

            private IQueryable<DeliveryMethodResource> FilterByPrice(IQueryable<DeliveryMethodResource> deliveryMethods, double? filter)
            {
                if (filter != null)
                {
                    deliveryMethods = deliveryMethods.Where(p => p.Price.CompareTo(filter.Value) == 0);
                }

                return deliveryMethods;
            }
        }
    }
}