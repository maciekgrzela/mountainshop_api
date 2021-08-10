using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;

namespace Application.Comment
{
    public class CreateComment
    {
        public class Command : IRequest
        {
            public string Content { get; set; }
            public string UserId { get; set; }
            public Guid ProductId { get; set; }
            public double Rate { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Content).NotEmpty();
                RuleFor(p => p.Rate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(5);
                RuleFor(p => p.UserId).NotEmpty();
                RuleFor(p => p.ProductId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IUnitOfWork _unitOfWork;
            private readonly UserManager<Domain.Models.User> _userManager;

            public Handler(DataContext context, IUnitOfWork unitOfWork, UserManager<Domain.Models.User> userManager)
            {
                _context = context;
                _unitOfWork = unitOfWork;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var existingUser = await _userManager.FindByIdAsync(request.UserId);

                if (existingUser == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono użytkownika dla podanego identyfikatora"});
                }

                var existingProduct = await _context.Products.FindAsync(request.ProductId);

                if (existingProduct == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }

                var comment = new Domain.Models.Comment
                {
                    Id = Guid.NewGuid(),
                    Content = request.Content,
                    Created = DateTime.Now,
                    Likes = 0,
                    Dislikes = 0,
                    Rate = request.Rate,
                    Product = existingProduct,
                    User = existingUser
                };

                await _context.Comments.AddAsync(comment, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}