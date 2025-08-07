using AutoMapper;
using PointOfSaleSystem.DTOs.Settings;
using PointOfSaleSystem.Models.Settings;

namespace PointOfSaleSystem.Profiles
{
    public class SettingsProfile : Profile
    {
        public SettingsProfile()
        {
            // BusinessProfile mappings
            CreateMap<BusinessProfile, BusinessProfileReadDto>();
            CreateMap<BusinessProfileCreateDto, BusinessProfile>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now));
            CreateMap<BusinessProfileUpdateDto, BusinessProfile>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now));

            //VAT Settings mappings
            CreateMap<VatSetting, VatSettingReadDto>();
            CreateMap<VatSettingCreateDto, VatSetting>();
            CreateMap<VatSettingUpdateDto, VatSetting>();

            // Discount Settings mappings
            CreateMap<DiscountSetting, DiscountSettingReadDto>();

            CreateMap<DiscountSettingCreateDto, DiscountSetting>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true)); // default to active on create

            CreateMap<DiscountSettingUpdateDto, DiscountSetting>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now));

            //Counter Settings mappings
            CreateMap<Counter, CounterReadDto>();
            CreateMap<CounterCreateDto, Counter>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
            CreateMap<CounterUpdateDto, Counter>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Receipt Settings mappings
            CreateMap<ReceiptSetting, ReceiptSettingReadDto>();

            CreateMap<ReceiptSettingCreateDto, ReceiptSetting>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            CreateMap<ReceiptSettingUpdateDto, ReceiptSetting>();
        }
    }
}
