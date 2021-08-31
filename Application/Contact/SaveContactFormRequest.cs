using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Contact
{
    public class SaveContactFormRequest
    {
        public class Command : IRequest
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Content { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.FirstName)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Imie nie może być puste")
                    .MaximumLength(100).WithMessage("Pole Imie nie może posiadać więcej niż 100 znaków");
                RuleFor(p => p.LastName)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Nazwisko nie może być puste")
                    .MaximumLength(100).WithMessage("Pole Nazwisko nie może posiadać więcej niż 100 znaków");
                RuleFor(p => p.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Email nie może być puste")
                    .EmailAddress().WithMessage("Pole Email musi posiadać odpowiedni format")
                    .MaximumLength(100).WithMessage("Pole Email nie może posiadać więcej niż 100 znaków");
                RuleFor(p => p.Content)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Zawartość nie może być puste")
                    .MaximumLength(1000).WithMessage("Pole Zawartość nie może posiadać więcej niż 1000 znaków");
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
                var contactRequest = new ContactRequest
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Content = request.Content,
                    Email = request.Email,
                    Created = DateTime.Now
                };

                await _context.ContactRequests.AddAsync(contactRequest, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}