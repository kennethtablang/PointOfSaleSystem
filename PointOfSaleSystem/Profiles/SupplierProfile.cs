using AutoMapper;

namespace PointOfSaleSystem.Profiles
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            CreateMap<Models.Suppliers.Supplier, DTOs.Suppliers.SupplierReadDto>();
            CreateMap<DTOs.Suppliers.SupplierCreateDto, Models.Suppliers.Supplier>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
            CreateMap<DTOs.Suppliers.SupplierUpdateDto, Models.Suppliers.Supplier>();
        }
    }
}
