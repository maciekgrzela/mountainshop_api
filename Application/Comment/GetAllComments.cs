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

                comments = FilterByUserId(comments, request.QueryParams);
                comments = FilterByProductId(comments, request.QueryParams);

                var commentsList = await PagedList<CommentResource>.ToPagedListAsync(comments, request.QueryParams.PageNumber,
                    request.QueryParams.PageSize);

                return commentsList;
            }

            private static IQueryable<CommentResource> FilterByProductId(IQueryable<CommentResource> comments, CommentParams requestQueryParams)
            {
                if (requestQueryParams.ProductId != Guid.Empty)
                {
                    comments = comments.Where(p => requestQueryParams.ProductId.ToString().Equals(p.Product.Id.ToString()));
                }

                return comments;
            }

            private static IQueryable<CommentResource> FilterByUserId(IQueryable<CommentResource> comments, CommentParams requestQueryParams)
            {
                if (!string.IsNullOrWhiteSpace(requestQueryParams.UserId))
                {
                    comments = comments.Where(p => requestQueryParams.UserId.Equals(p.User.Id));
                }

                return comments;
            }
        }
    }
}