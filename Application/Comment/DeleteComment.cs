using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Comment
{
    public class DeleteComment
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _dataContext;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(DataContext dataContext, IUnitOfWork unitOfWork)
            {
                _dataContext = dataContext;
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingComment = await _dataContext.Comments.FindAsync(request.Id);

                if (existingComment == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        "Nie znaleziono komentarza dla podanego identyfikatora");
                }

                _dataContext.Comments.Remove(existingComment);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
    }
}