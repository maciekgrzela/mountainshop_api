using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Comment.Resources;
using Application.Errors;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Comment
{
    public class GetCommentsForProduct
    {
        public class Query : IRequest<List<CommentResource>>
        {
            public Guid Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id).NotEmpty();
            }
        }

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
                var product = await _context.Products.FindAsync(request.Id);

                if (product == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }

                var comments = await _context.Comments.Where(p => p.ProductId == request.Id)
                    .ToListAsync(cancellationToken: cancellationToken);

                var commentsResource = _mapper.Map<List<Domain.Models.Comment>, List<CommentResource>>(comments);

                return commentsResource;
            }
        }
    }
}