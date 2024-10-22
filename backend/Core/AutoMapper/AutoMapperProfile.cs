using AutoMapper;
using backend.Core.Dtos;
using backend.Core.Entities;

namespace backend.Core.AutoMapper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductGetDTO>();
            CreateMap<ProductCreateDTO, Product>();
        }
    }
}
