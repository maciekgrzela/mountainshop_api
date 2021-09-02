using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Errors;
using Application.Product.Resources;
using AutoMapper;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.ProductsProperty
{
    public class GetPropertiesForProduct
    {
        public class Query : IRequest<List<PropertyValueForProductResource>>
        {
            public Guid Id { get; set; }
        }
        
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Pole Identyfikator nie może być puste");
            }
        }
        
        public class Handler : IRequestHandler<Query, List<PropertyValueForProductResource>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            
            public async Task<List<PropertyValueForProductResource>> Handle(Query request, CancellationToken cancellationToken)
            {
                var existingProduct = await _context.Products.FindAsync(request.Id);

                if (existingProduct == null)
                {
                    throw new RestException(HandlerResponse.ResourceNotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }

                var properties = await _context.ProductsPropertyValues
                    .Include(p => p.ProductsProperty)
                    .Where(p => p.ProductId == request.Id)
                    .ToListAsync(cancellationToken: cancellationToken);

                var propertiesResource =
                    _mapper.Map<List<ProductsPropertyValue>, List<PropertyValueForProductResource>>(properties);

                return propertiesResource;
            }
        }
    }
}