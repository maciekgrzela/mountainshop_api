using System.Linq;
using Application.Category;
using Application.Comment.Resources;
using Application.Complaint.Resources;
using Application.DeliveryMethod.Resources;
using Application.Order.Resources;
using Application.PaymentMethod.Resources;
using Application.Producer.Resources;
using Application.Product;
using Application.Product.Resources;
using Application.ProductsProperty.Resources;
using Application.User;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<Domain.Models.User, LoggedUserResource>();
            CreateMap<Domain.Models.Category, CategoryResource>();
            CreateMap<PagedList<Domain.Models.Comment>, PagedList<CommentResource>>();
            CreateMap<Domain.Models.Producer, ProducerResource>();
            CreateMap<Domain.Models.Product, ProductResource>();
            CreateMap<Domain.Models.Producer, ProducerForProductResource>();
            CreateMap<Domain.Models.Comment, CommentResource>();
            CreateMap<Domain.Models.User, UserForCommentsResource>();
            CreateMap<Domain.Models.Product, ProductForCommentsResource>();
            CreateMap<Domain.Models.Complaint, ComplaintResource>();
            CreateMap<Domain.Models.Order, OrderForComplaintResource>();
            CreateMap<Domain.Models.Category, CategoryForProductResource>();
            CreateMap<Domain.Models.Comment, CommentForProductResource>();
            CreateMap<ProductsPropertyValue, PropertyValueForProductResource>();
            CreateMap<Domain.Models.Product, SingleProductResource>();
            CreateMap<Domain.Models.User, UserForProductsCommentResource>();
            CreateMap<Domain.Models.Product, ProductWithCommentsResource>();
            CreateMap<ProductWithCommentsResource, ProductResource>();
            CreateMap<Domain.Models.ProductsProperty, ProductsPropertyResource>();
            CreateMap<Domain.Models.PaymentMethod, PaymentMethodResource>();
            CreateMap<Domain.Models.DeliveryMethod, DeliveryMethodResource>();
            CreateMap<Domain.Models.DeliveryMethod, DeliveryMethodForPaymentResource>();
            CreateMap<Domain.Models.PaymentMethod, PaymentMethodForDeliveryResource>();
            CreateMap<OrderedProduct, OrderedProductForOrderResource>();
            CreateMap<Domain.Models.Order, OrderResource>();
            CreateMap<OrderDetails, OrderDetailsForOrderResource>();
            CreateMap<Domain.Models.PaymentMethod, PaymentMethodForOrderResource>();
            CreateMap<Domain.Models.DeliveryMethod, DeliveryMethodForOrderResource>();
            CreateMap<Domain.Models.Order, OrderForUserResource>();
            CreateMap<CreateProduct.ProductsPropertyCommand, ProductsPropertyValue>();
            CreateMap<OrderedProduct, OrderedProductForUserOrderResource>()
                .ForMember(p => p.Description, o => o.MapFrom(p => p.Product.Description))
                .ForMember(p => p.Image, o => o.MapFrom(p => p.Product.Image))
                .ForMember(p => p.Name, o => o.MapFrom(p => p.Product.Name))
                .ForMember(p => p.GrossPrice, o => o.MapFrom(p => p.Product.GrossPrice))
                .ForMember(p => p.NetPrice, o => o.MapFrom(p => p.Product.NetPrice))
                .ForMember(p => p.PercentageSale, o => o.MapFrom(p => p.Product.PercentageSale))
                .ForMember(p => p.PercentageTax, o => o.MapFrom(p => p.Product.PercentageTax));
        }
    }
}