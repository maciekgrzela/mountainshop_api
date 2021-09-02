using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Comment.Validators;
using Application.Core;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Comment
{
    public class ModifyCommentsPopularity
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string Vote { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
                RuleFor(p => p.Vote)
                    .Must(CommentCustomValidators.ValidVote).WithMessage("Pole Głos musi posiadać jedną z dozwolonych wartości (like/dislike)");
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(DataContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }
            
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingComment = await _context.Comments.FindAsync(request.Id);

                if (existingComment == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono komentarza dla podanego identyfikatora"});
                }

                switch (request.Vote)
                {
                    case "like":
                        existingComment.Likes += 1;
                        break;
                    case "dislike":
                        existingComment.Dislikes += 1;
                        break;
                }
                
                _context.Comments.Update(existingComment);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}