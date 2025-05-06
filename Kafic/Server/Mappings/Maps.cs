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
        }
    }
}
