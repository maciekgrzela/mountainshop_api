﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.DeliveryMethod
{
    public class UpdateDeliveryMethod
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public double Price { get; set; }
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
                    .NotEmpty().WithMessage("Pole Imie nie może być puste")
                    .MinimumLength(5).WithMessage("Pole Imię musi posiadać więcej niż 5 znaków");
                RuleFor(p => p.Price)
                    .NotEmpty().WithMessage("Pole Cena nie może być puste")
                    .GreaterThanOrEqualTo(0).WithMessage("Pole Cena musi być większe lub równe zero");
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
                var existingDeliveryMethod = await _context.DeliveryMethods.FindAsync(request.GetId());

                if (existingDeliveryMethod == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono metody dostawy dla podanego identyfikatora"});
                }

                existingDeliveryMethod.Name = request.Name;
                existingDeliveryMethod.Price = request.Price;

                _context.DeliveryMethods.Update(existingDeliveryMethod);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}