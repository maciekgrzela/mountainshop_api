using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Producer.Params;
using Application.Producer.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Producer
{
    public class GetAllProducers
    {
        public class Query : IRequest<PagedList<ProducerResource>>
        {
            public ProducerParams QueryParams { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedList<ProducerResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<PagedList<ProducerResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var producers = _context.Producers
                    .Include(p => p.Products)
                    .OrderByDescending(p => p.Products.Count)
                    .ProjectTo<ProducerResource>(_mapper.ConfigurationProvider).AsQueryable();


                producers = FilterByName(producers, request.QueryParams.NameFilter);
                producers = FilterByDescription(producers, request.QueryParams.DescriptionFilter);
                producers = SortByName(producers, request.QueryParams.NameSort);
                producers = SortByDescription(producers, request.QueryParams.DescriptionSort);

                var producersList = await PagedList<ProducerResource>.ToPagedListAsync(producers,
                    request.QueryParams.PageNumber, request.QueryParams.PageSize);
                

                return producersList;
            }

            private IQueryable<ProducerResource> SortByDescription(IQueryable<ProducerResource> producers, bool? filter)
            {
                if (filter != null)
                {
                    producers = filter.Value
                        ? producers.OrderBy(p => p.Description)
                        : producers.OrderByDescending(p => p.Description);
                }

                return producers;
            }

            private IQueryable<ProducerResource> SortByName(IQueryable<ProducerResource> producers, bool? filter)
            {
                if (filter != null)
                {
                    producers = filter.Value
                        ? producers.OrderBy(p => p.Name)
                        : producers.OrderByDescending(p => p.Name);
                }

                return producers;
            }

            private IQueryable<ProducerResource> FilterByDescription(IQueryable<ProducerResource> producers, string filter)
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    producers = producers.Where(p => p.Description.Contains(filter));
                }

                return producers;
            }

            private IQueryable<ProducerResource> FilterByName(IQueryable<ProducerResource> producers, string filter)
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    producers = producers.Where(p => p.Name.Contains(filter));
                }

                return producers;
            }
        }
    }
}