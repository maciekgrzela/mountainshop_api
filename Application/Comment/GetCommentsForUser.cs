using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Comment.Resources;
using Application.Core;
using Application.Errors;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Comment
{
    public class GetCommentsForUser
    {
        public class Query : IRequest<List<CommentResource>>
        {
            public string Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
            }
        }

        public class Handler : IRequestHandler<Query, List<CommentResource>>
        {
            private readonly DataContext _context;
            private readonly UserManager<Domain.Models.User> _userManager;
            private readonly IMapper _mapper;

            public Handler(DataContext context, UserManager<Domain.Models.User> userManager, IMapper mapper)
            {
                _context = context;
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<List<CommentResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.Id);

                if (user == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono użytkownika dla podanego identyfikatora"});
                }

                var comments = await _context.Comments.Where(p => p.UserId == request.Id)
                    .ToListAsync(cancellationToken: cancellationToken);

                var commentsResource = _mapper.Map<List<Domain.Models.Comment>, List<CommentResource>>(comments);

                return commentsResource;
            }
        }
    }
}