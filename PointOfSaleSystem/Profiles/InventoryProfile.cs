using AutoMapper;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Profiles
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            // Category mappings
            CreateMap<Category, CategoryViewDto>();
            CreateMap<CategoryCreateDto, Category>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<CategoryUpdateDto, Category>();

            // Product Mappings
            CreateMap<Product, ProductViewDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit!.Name));

            CreateMap<Product, ProductViewDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name))
                .ForMember(dest => dest.ImageBase64, opt => opt.MapFrom(src => src.ImageData != null ? Convert.ToBase64String(src.ImageData) : null));

            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.ImageBase64) ? Convert.FromBase64String(src.ImageBase64!) : null));

            CreateMap<ProductUpdateDto, Product>();


            CreateMap<Unit, UnitViewDto>();
            CreateMap<UnitCreateDto, Unit>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UnitUpdateDto, Unit>();


            // ProductUnitConversion mappings
            CreateMap<ProductUnitConversion, ProductUnitConversionReadDto>()
                .ForMember(dest => dest.FromUnitName,
                           opt => opt.MapFrom(src => src.FromUnit != null ? src.FromUnit.Name : null))
                .ForMember(dest => dest.ToUnitName,
                           opt => opt.MapFrom(src => src.ToUnit != null ? src.ToUnit.Name : null))
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));

            CreateMap<ProductUnitConversionCreateDto, ProductUnitConversion>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.FromUnit, opt => opt.Ignore())
                .ForMember(dest => dest.ToUnit, opt => opt.Ignore());

            CreateMap<ProductUnitConversionUpdateDto, ProductUnitConversion>()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.FromUnit, opt => opt.Ignore())
                .ForMember(dest => dest.ToUnit, opt => opt.Ignore());
        }
    }
}
