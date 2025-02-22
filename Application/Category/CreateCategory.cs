﻿using System;
using System.Data;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using Application.Photo;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence.Context;

namespace Application.Category
{
    public class CreateCategory
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public IFormFile Image { get; set; }
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
            private readonly IPhotoAccessor _photoAccessor;

            public Handler(DataContext context, IUnitOfWork unitOfWork, IPhotoAccessor photoAccessor)
            {
                _context = context;
                _unitOfWork = unitOfWork;
                _photoAccessor = photoAccessor;
            }
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                PhotoUploadResult uploadedImage = null;
                if (request.Image != null)
                {
                    try
                    {
                        uploadedImage = _photoAccessor.AddPhoto(request.Image);
                    }
                    catch (Exception e)
                    {
                        throw new RestException(HandlerResponse.InvalidRequest, new {info = e.Message});
                    }
                }
                
                
                var category = new Domain.Models.Category
                {
                    Id = Guid.NewGuid(),
                    Description = request.Description,
                    Name = request.Name,
                    Image = uploadedImage?.Url,
                };

                await _context.Categories.AddAsync(category, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();
                return await Task.FromResult(Unit.Value);
            }
        }
        
    }
}