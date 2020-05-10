using AutoMapper;
using Homer.Models.Domain;
using Homer.Models.DTO;

namespace Homer.Models.Profiles
{
    public class StoreProfile : Profile
    {
        public StoreProfile() 
        {
            CreateMap<StoreDto, Store>();
            CreateMap<Store, StoreDto>();
            CreateMap<AddressInfoDto, AddressInfo>();
            CreateMap<AddressInfo, AddressInfoDto>();
            CreateMap<LocationDto, Location>();
            CreateMap<Location, LocationDto>();
            CreateMap<BusinessHourDto, BusinessHour>();
            CreateMap<BusinessHour, BusinessHourDto>();
            CreateMap<CompanyDto, Company>();
            CreateMap<Company, CompanyDto>();

        }
    }
}
