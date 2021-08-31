using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Order.Resources;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Order
{
    public class GetLastOrderForUser
    {
        public class Query : IRequest<OrderForUserResource>
        {
            public string Id { get; set; }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
            }
        }
        
        public class Handler : IRequestHandler<Query, OrderForUserResource>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly UserManager<Domain.Models.User> _userManager;

            public Handler(DataContext context, IMapper mapper, UserManager<Domain.Models.User> userManager)
            {
                _context = context;
                _mapper = mapper;
                _userManager = userManager;
            }
            
            
            public async Task<OrderForUserResource> Handle(Query request, CancellationToken cancellationToken)
            {
                var existingUser = await _userManager.FindByIdAsync(request.Id);

                if (existingUser == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono użytkownika dla podanego identyfikatora"});
                }

                var order = await _context.Orders
                    .Include(p => p.OrderDetails)
                    .Include(p => p.DeliveryMethod)
                    .Include(p => p.PaymentMethod)
                    .Include(p => p.OrderedProducts)
                    .ThenInclude(p => p.Product)
                    .OrderByDescending(p => p.Created)
                    .FirstOrDefaultAsync(p => p.OrderDetails.UserId == request.Id, cancellationToken: cancellationToken);

                if (order == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono zamówienia dla podanego identyfikatora"});
                }

                var orderResource = _mapper.Map<Domain.Models.Order, OrderForUserResource>(order);
                return orderResource;
            }
        }
    }
}