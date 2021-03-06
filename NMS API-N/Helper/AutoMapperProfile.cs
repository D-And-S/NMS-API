using AutoMapper;
using NMS_API_N.Model.DTO;
using NMS_API_N.Model.Entities;

namespace NMS_API_N.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterDto, User>().ReverseMap();

            CreateMap<RoleDto, Role>().ReverseMap();

            CreateMap<CompanyDto, Company>().ReverseMap();

            CreateMap<AddressDto, Address>().ReverseMap();

            CreateMap<CountryDto, Country>().ReverseMap();

            CreateMap<CityDto, City>().ReverseMap();

            CreateMap<string, string>().ConvertUsing(new StringTrimmerProfile());
        }
    }
}
