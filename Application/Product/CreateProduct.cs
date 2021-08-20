using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Photo;
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Application.Product
{
    public class CreateProduct
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<ProductsPropertyCommand> ProductsPropertyValues { get; set; }
            public int AmountInStorage { get; set; }
            public double NetPrice { get; set; }
            public double PercentageTax { get; set; }
            public int MinimalOrderedAmount { get; set; }
            public Guid ProducerId { get; set; }
            public Guid CategoryId { get; set; }
        }
        
        public class ProductsPropertyCommand
        {
            public Guid ProductsPropertyId { get; set; }
            public string Value { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly DataContext _context;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IUnitOfWork unitOfWork, IPhotoAccessor photoAccessor, IMapper mapper)
            {
                _context = context;
                _unitOfWork = unitOfWork;
                _photoAccessor = photoAccessor;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var producer = await _context.Producers.FindAsync(request.ProducerId);

                if (producer == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono producenta dla podanego identyfikatora"});
                }

                var category = await _context.Categories.FindAsync(request.CategoryId);

                if (category == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono kategorii dla podanego identyfikatora"});
                }

                var existingPropertiesIds = await _context.ProductsProperties.Select(p => p.Id).ToListAsync(cancellationToken: cancellationToken);
                var propertiesIds = request.ProductsPropertyValues.Select(p => p.ProductsPropertyId).ToList();

                var count = existingPropertiesIds.Intersect(propertiesIds).Count();
                if (count != propertiesIds.Count)
                {
                    throw new RestException(HttpStatusCode.BadRequest,
                        new {info = "Identyfikatory niektórych właściwości produktu są niepoprawne"});
                }

                var productsPropertyValues =
                    _mapper.Map<List<ProductsPropertyCommand>, List<ProductsPropertyValue>>(request.ProductsPropertyValues);

                var product = new Domain.Models.Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    AmountInStorage = request.AmountInStorage,
                    Comments = new List<Domain.Models.Comment>(),
                    Description = request.Description,
                    Producer = producer,
                    Category = category,
                    NetPrice = request.NetPrice,
                    PercentageTax = request.PercentageTax,
                    GrossPrice = request.NetPrice + (request.NetPrice * (request.PercentageTax / 100)),
                    MinimalOrderedAmount = request.MinimalOrderedAmount,
                    ProductsPropertyValues = productsPropertyValues,
                };

                await _context.Products.AddAsync(product, cancellationToken);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}