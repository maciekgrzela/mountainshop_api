using Application.Category;
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
        }
    }
}