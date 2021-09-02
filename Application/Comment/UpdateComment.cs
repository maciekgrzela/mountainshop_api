using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Comment
{
    public class UpdateComment
    {
        public class Command : IRequest
        {
            public string Content { get; set; }
            public double Rate { get; set; }
            private Guid _id;

            public void SetId(Guid id)
            {
                _id = id;
            }

            public Guid GetId()
            {
                return _id;
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Content)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Treść nie może być puste")
                    .MaximumLength(1000).WithMessage("Pole Treść nie może zawierać więcej niż 1000 znaków");

                RuleFor(p => p.Rate)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Głos nie może być puste")
                    .GreaterThanOrEqualTo(0).WithMessage("Wartość pola Głos musi być większa lub równa 0")
                    .LessThanOrEqualTo(5).WithMessage("Wartość pola Głos musi być mniejsza lub równa 5");
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
                var existingComment = await _context.Comments.FindAsync(request.GetId());

                if (existingComment == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono komentarza dla podanego identyfikatora"});
                }

                existingComment.Content = request.Content;
                existingComment.Rate = request.Rate;

                _context.Comments.Update(existingComment);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}