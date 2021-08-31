using System;
using System.Data;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Photo;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence.Context;

namespace Application.Product
{
    public class UploadProductsImage
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public IFormFile Image { get; set; }
        }
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
                RuleFor(p => p.Image)
                    .NotEmpty().WithMessage("Pole Zdjęcie nie może być puste");
            }
        }
        
        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUnitOfWork unitOfWork)
            {
                _context = context;
                _photoAccessor = photoAccessor;
                _unitOfWork = unitOfWork;
            }
            
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingProduct = await _context.Products.FindAsync(request.Id);

                if (existingProduct == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }
                
                PhotoUploadResult uploadedImage;
                
                try
                {
                    uploadedImage = _photoAccessor.AddPhoto(request.Image);
                }
                catch (Exception e)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new {info = e.Message});
                }

                existingProduct.Image = uploadedImage.Url;

                _context.Products.Update(existingProduct);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}