using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Category
{
    public class CreateCategory
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public string ImagePath { get; set; }
            public string Description { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Name).NotEmpty();
                RuleFor(p => p.ImagePath).NotEmpty();
                RuleFor(p => p.Description).NotEmpty();
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
                var category = new Domain.Models.Category
                {
                    Id = Guid.NewGuid(),
                    Description = request.Description,
                    ImagePath = request.ImagePath,
                    Name = request.Name
                };

                await _context.Categories.AddAsync(category, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
        
    }
}