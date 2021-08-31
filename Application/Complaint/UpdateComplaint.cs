using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Compliant
{
    public class UpdateCompliant
    {
        public class Command : IRequest
        {
            public string Abbreviation { get; set; }
            public string Description { get; set; }
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
                RuleFor(p => p.Abbreviation)
                    .NotEmpty().WithMessage("Pole Krótki Opis nie może być puste");
                
                RuleFor(p => p.Description)
                    .NotEmpty().WithMessage("Pole Opis nie może być puste");
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
                var existingComplaint = await _context.Complaints.FindAsync(request.GetId());

                if (existingComplaint == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono reklamacji dla podanego identyfikatora"});
                }

                existingComplaint.Abbreviation = request.Abbreviation;
                existingComplaint.Description = request.Description;

                _context.Complaints.Update(existingComplaint);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}