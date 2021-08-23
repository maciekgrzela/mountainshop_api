using System.Linq;
using Application.Category;
using Application.Comment.Resources;
using Application.Complaint.Resources;
using Application.DeliveryMethod.Resources;
using Application.PaymentMethod.Resources;
using Application.Producer.Resources;
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
        }
    }
}