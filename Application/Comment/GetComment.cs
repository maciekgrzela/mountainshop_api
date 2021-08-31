using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Comment.Resources;
using Application.Errors;
using AutoMapper;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Comment
{
    public class GetComment
    {
        public class Query : IRequest<CommentResource>
        {
            public Guid Id { get; set; }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
            }
        }

        public class Handler : IRequestHandler<Query, CommentResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<CommentResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var comment = await _context.Comments.FindAsync(request.Id);

                if (comment == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono komentarza dla podanego identyfikatora"});
                }

                var commentResource = _mapper.Map<Domain.Models.Comment, CommentResource>(comment);

                return commentResource;
            }
        }
    }
}