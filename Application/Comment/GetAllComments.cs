using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Comment.Resources;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Comment
{
    public class GetAllComments
    {
        public class Query : IRequest<List<CommentResource>> { }

        public class Handler : IRequestHandler<Query, List<CommentResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            
            public async Task<List<CommentResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var comments = await _context.Comments.ToListAsync(cancellationToken);
                var commentsResource = _mapper.Map<List<Domain.Models.Comment>, List<CommentResource>>(comments);

                return commentsResource;
            }
        }
    }
}