using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Category;
using Application.Order.Params;
using Application.Order.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Order
{
    public class GetAllOrders
    {
        public class Query : IRequest<PagedList<OrderResource>>
        {
            public OrderParams QueryParams { get; set; }
        }
        
        public class QueryHandler : IRequestHandler<Query, PagedList<OrderResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PagedList<OrderResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var orders = _context.Orders
                    .Include(p => p.DeliveryMethod)
                    .Include(p => p.PaymentMethod)
                    .Include(p => p.OrderDetails)
                    .Include(p => p.OrderedProducts)
                    .OrderByDescending(p => p.Created)
                    .ProjectTo<OrderResource>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                orders = FilterByNumber(orders, request.QueryParams.NumberFilter);
                orders = SortByNumber(orders, request.QueryParams.NumberSort);
                orders = FilterByPaymentMethod(orders, request.QueryParams.PaymentMethodFilter);
                orders = SortByPaymentMethod(orders, request.QueryParams.PaymentMethodSort);
                orders = FilterByDeliveryMethod(orders, request.QueryParams.DeliveryMethodFilter);
                orders = SortByDeliveryMethod(orders, request.QueryParams.DeliveryMethodSort);
                orders = FilterByStatus(orders, request.QueryParams.StatusFilter);
                orders = SortByStatus(orders, request.QueryParams.StatusSort);
                
                
                
                return await PagedList<OrderResource>.ToPagedListAsync(orders, request.QueryParams.PageNumber, request.QueryParams.PageSize);
            }

            private IQueryable<OrderResource> SortByStatus(IQueryable<OrderResource> orders, bool? queryParamsStatusSort)
            {
                if (queryParamsStatusSort != null)
                {
                    orders = queryParamsStatusSort.Value
                        ? orders.OrderBy(p => p.Status)
                        : orders.OrderByDescending(p => p.Status);
                }

                return orders;
            }

            private IQueryable<OrderResource> FilterByStatus(IQueryable<OrderResource> orders, string queryParamsStatusFilter)
            {
                if (!string.IsNullOrWhiteSpace(queryParamsStatusFilter))
                {
                    orders = orders.Where(p => p.Status.Contains(queryParamsStatusFilter));
                }

                return orders;
            }

            private IQueryable<OrderResource> SortByDeliveryMethod(IQueryable<OrderResource> orders, bool? queryParamsDeliveryMethodSort)
            {
                if (queryParamsDeliveryMethodSort != null)
                {
                    orders = queryParamsDeliveryMethodSort.Value
                        ? orders.OrderBy(p => p.DeliveryMethod.Name)
                        : orders.OrderByDescending(p => p.DeliveryMethod.Name);
                }

                return orders;
            }

            private IQueryable<OrderResource> FilterByDeliveryMethod(IQueryable<OrderResource> orders, Guid? queryParamsDeliveryMethodFilter)
            {
                if (queryParamsDeliveryMethodFilter != null)
                {
                    orders = orders.Where(p => p.DeliveryMethod.Id.ToString().Equals(queryParamsDeliveryMethodFilter.ToString()));
                }

                return orders;
            }

            private IQueryable<OrderResource> SortByPaymentMethod(IQueryable<OrderResource> orders, bool? queryParamsPaymentMethodSort)
            {
                if (queryParamsPaymentMethodSort != null)
                {
                    orders = queryParamsPaymentMethodSort.Value
                        ? orders.OrderBy(p => p.PaymentMethod.Name)
                        : orders.OrderByDescending(p => p.PaymentMethod.Name);
                }

                return orders;
            }

            private IQueryable<OrderResource> FilterByPaymentMethod(IQueryable<OrderResource> orders, Guid? queryParamsPaymentMethodFilter)
            {
                if (queryParamsPaymentMethodFilter != null)
                {
                    orders = orders.Where(p => p.PaymentMethod.Id.ToString().Equals(queryParamsPaymentMethodFilter.ToString()));
                }

                return orders;
            }

            private IQueryable<OrderResource> SortByNumber(IQueryable<OrderResource> orders, bool? queryParamsNumberSort)
            {
                if (queryParamsNumberSort != null)
                {
                    orders = queryParamsNumberSort.Value
                        ? orders.OrderBy(p => p.Number)
                        : orders.OrderByDescending(p => p.Number);
                }

                return orders;
            }

            private IQueryable<OrderResource> FilterByNumber(IQueryable<OrderResource> orders, int? queryParamsNumberFilter)
            {
                if (queryParamsNumberFilter != null)
                {
                    orders = orders.Where(p => p.Number == queryParamsNumberFilter);
                }

                return orders;
            }
        }
    }
}