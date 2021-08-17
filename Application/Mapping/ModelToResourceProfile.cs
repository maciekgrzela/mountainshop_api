using Application.Category;
using Application.Comment.Resources;
using Application.Complaint.Resources;
using Application.Producer.Resources;
using Application.Product.Resources;
using Application.User;
using AutoMapper;

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
        }
    }
}