﻿using System.Collections.Generic;
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

                deliveryMethods = FilterByPrice(deliveryMethods, request.QueryParams);
                deliveryMethods = SortByPrice(deliveryMethods, request.QueryParams);

                var deliveryMethodsList = await PagedList<DeliveryMethodResource>.ToPagedListAsync(deliveryMethods,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);

                return deliveryMethodsList;
            }

            private IQueryable<DeliveryMethodResource> SortByPrice(IQueryable<DeliveryMethodResource> deliveryMethods, DeliveryMethodParams requestQueryParams)
            {
                if (requestQueryParams.PriceAsc != null)
                {
                    deliveryMethods = deliveryMethods.OrderBy(p => p.Price);
                }

                if (requestQueryParams.PriceDesc != null)
                {
                    deliveryMethods = deliveryMethods.OrderByDescending(p => p.Price);
                }

                return deliveryMethods;
            }

            private IQueryable<DeliveryMethodResource> FilterByPrice(IQueryable<DeliveryMethodResource> deliveryMethods, DeliveryMethodParams requestQueryParams)
            {
                if (requestQueryParams.PriceFilter != null)
                {
                    deliveryMethods = deliveryMethods.Where(p =>
                        requestQueryParams.PriceFilter != null && p.Price.CompareTo(requestQueryParams.PriceFilter) == 0);
                }

                return deliveryMethods;
            }
        }
    }
}