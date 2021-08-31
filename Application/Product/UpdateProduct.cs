using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Product.Validators;
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
            public string Gender { get; set; }
            public double PercentageTax { get; set; }
            public int MinimalOrderedAmount { get; set; }
            public Guid ProducerId { get; set; }
            public Guid CategoryId { get; set; }
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
                    .MinimumLength(5).WithMessage("Pole Nazwa musi posiadać co najmniej 5 znaków")
                    .MaximumLength(20).WithMessage("Pole Nazwa może posiadać co najwyżej 20 znaków");
                
                RuleFor(p => p.Description)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Opis nie może być puste")
                    .MaximumLength(2000).WithMessage("Pole Opis może posiadać co najwyżej 2000 znaków");
                
                RuleFor(p => p.AmountInStorage)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Stan Magazynowy nie może być puste")
                    .GreaterThanOrEqualTo(0).WithMessage("Wartość pola Stan Magazynowy musi być większa od zera");
                
                RuleFor(p => p.NetPrice)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Cena Netto nie może być puste")
                    .GreaterThanOrEqualTo(0).WithMessage("Wartość pola Cena Netto musi być większa lub równa zero");
                
                RuleFor(p => p.PercentageTax)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Podatek nie może być puste")
                    .GreaterThanOrEqualTo(0).WithMessage("Wartość pola Podatek musi być większa lub równa zero")
                    .LessThanOrEqualTo(100).WithMessage("Wartość pola Podatek musi być mniejsza lub równa sto");
                
                RuleFor(p => p.Gender)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Płeć nie może być puste")
                    .Must(ProductsCustomValidators.GenderIsValid).WithMessage("Pole Płeć musi posiadać jedną z dozwolonych wartości (male/female/unisex)");
                
                RuleFor(p => p.MinimalOrderedAmount)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Pole Minimalne Zamówienie nie może być puste")
                    .GreaterThanOrEqualTo(0).WithMessage("Wartość pola Minimalne Zamówienie musi być większa lub równa zero");
                
                RuleFor(p => p.ProducerId)
                    .NotEmpty().WithMessage("Pole Producent nie może być puste");
                RuleFor(p => p.CategoryId)
                    .NotEmpty().WithMessage("Pole Kategoria nie może być puste");;
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
                var existingProducer = await _context.Producers.FindAsync(request.ProducerId);

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
                existingProduct.Gender = request.Gender;
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