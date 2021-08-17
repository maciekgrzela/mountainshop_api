using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence.Context;

namespace Application.Product
{
    public class UpdateProduct
    {
        public class Command : IRequest
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int AmountInStorage { get; set; }
            public double NetPrice { get; set; }
            public double PercentageTax { get; set; }
            public int MinimalOrderedAmount { get; set; }
            public Guid ProducerId { get; set; }
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
                RuleFor(p => p.Name).NotEmpty();
                RuleFor(p => p.Description).NotEmpty();
                RuleFor(p => p.AmountInStorage).GreaterThanOrEqualTo(0).NotEmpty();
                RuleFor(p => p.NetPrice).GreaterThanOrEqualTo(0).NotEmpty();
                RuleFor(p => p.PercentageTax).GreaterThanOrEqualTo(0).NotEmpty();
                RuleFor(p => p.MinimalOrderedAmount).GreaterThanOrEqualTo(0).NotEmpty();
                RuleFor(p => p.ProducerId).NotEmpty();
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
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono producenta dla podanego identyfikatora"});
                }

                var existingProduct = await _context.Products.FindAsync(request.GetId());
                
                if (existingProduct == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,
                        new {info = "Nie znaleziono produktu dla podanego identyfikatora"});
                }

                existingProduct.Name = request.Name;
                existingProduct.Description = request.Description;
                existingProduct.AmountInStorage = request.AmountInStorage;
                existingProduct.NetPrice = request.NetPrice;
                existingProduct.PercentageTax = request.PercentageTax;
                existingProduct.GrossPrice = request.NetPrice + (request.NetPrice * request.PercentageTax);
                existingProduct.MinimalOrderedAmount = request.MinimalOrderedAmount;
                existingProduct.Producer = existingProducer;

                _context.Products.Update(existingProduct);
                await _unitOfWork.CommitTransactionsAsync();

                return await Task.FromResult(Unit.Value);
            }
        }
    }
}