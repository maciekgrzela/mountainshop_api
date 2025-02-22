﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Producer
{
    public class UpdateProducer
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
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
                RuleFor(p => p.Name)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Nazwa nie może być puste")
                    .MinimumLength(5).WithMessage("Pole Nazwa musi posiadać co najmniej 5 znaków");
                
                RuleFor(p => p.Description)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Opis nie może być puste")
                    .MaximumLength(1000).WithMessage("Pole Opis może posiadać co najwyżej 1000 znaków");
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
                var existingProducer = await _context.Producers.FindAsync(request.GetId());

                if (existingProducer == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono producenta dla podanego identyfikatora"});
                }

                existingProducer.Name = request.Name;
                existingProducer.Description = request.Description;

                _context.Producers.Update(existingProducer);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}