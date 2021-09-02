using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Comment.Params;
using Application.Comment.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Context;

namespace Application.Comment
{
    public class GetAllComments
    {
        public class Query : IRequest<PagedList<CommentResource>>
        {
            public CommentParams QueryParams { get; set; }
        }

        public class Handler : IRequestHandler<Query, PagedList<CommentResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }
            
            
            public async Task<PagedList<CommentResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var comments = _context.Comments
                                                .Include(p => p.User)
                                                .Include(p => p.Product)
                                                .OrderBy(p => p.Created)
                                                .ProjectTo<CommentResource>(_mapper.ConfigurationProvider)
                                                .AsQueryable();

                comments = FilterByUser(comments, request.QueryParams.UserFilter);
                comments = SortByUser(comments, request.QueryParams.UserSort);
                comments = FilterByProduct(comments, request.QueryParams.ProductFilter);
                comments = SortByProduct(comments, request.QueryParams.ProductSort);

                var commentsList = await PagedList<CommentResource>.ToPagedListAsync(comments, request.QueryParams.PageNumber,
                    request.QueryParams.PageSize);

                return commentsList;
            }

            private IQueryable<CommentResource> SortByProduct(IQueryable<CommentResource> comments, bool? queryParamsProductSort)
            {
                if (queryParamsProductSort != null)
                {
                    comments = queryParamsProductSort.Value
                        ? comments.OrderBy(p => p.Product.Name)
                        : comments.OrderByDescending(p => p.Product.Name);
                }

                return comments;
            }

            private IQueryable<CommentResource> SortByUser(IQueryable<CommentResource> comments, bool? queryParamsUserSort)
            {
                if (queryParamsUserSort != null)
                {
                    comments = queryParamsUserSort.Value
                        ? comments.OrderBy(p => p.User.FirstName)
                        : comments.OrderByDescending(p => p.User.FirstName);
                }

                return comments;
            }

            private static IQueryable<CommentResource> FilterByProduct(IQueryable<CommentResource> comments, Guid? filter)
            {
                if (filter != null)
                {
                    comments = comments.Where(p => filter.ToString().Equals(p.Product.Id.ToString()));
                }

                return comments;
            }

            private static IQueryable<CommentResource> FilterByUser(IQueryable<CommentResource> comments, string filter)
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    comments = comments.Where(p => filter.Equals(p.User.Id));
                }

                return comments;
            }
        }
    }
}