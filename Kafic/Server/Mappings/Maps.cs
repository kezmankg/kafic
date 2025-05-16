using AutoMapper;
using Server.Data;
using Share.Models;

namespace Server.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Caffe, CompanyModel>().ReverseMap();
            CreateMap<ApplicationUser, RegistrationUserModel>().ReverseMap();
            CreateMap<ApplicationUser, RegistrationUserModelEdit>().ReverseMap();
            CreateMap<Group, GroupModel>().ReverseMap();
            CreateMap<Subgroup, SubgroupModel>().ReverseMap();
            CreateMap<Article, ArticleModel>().ReverseMap();
            CreateMap<Order, OrderModel>().ReverseMap();
            CreateMap<Order, OrderPaid>().ReverseMap();
            CreateMap<OrderArticle, OrderPaidArticle>().ReverseMap();
        }
    }
}
