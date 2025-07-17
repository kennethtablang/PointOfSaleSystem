using AutoMapper;
using PointOfSaleSystem.DTOs.Inventory.Category;
using PointOfSaleSystem.DTOs.Inventory.Product;
using PointOfSaleSystem.DTOs.Inventory.Unit;
using PointOfSaleSystem.Models.Inventory;

namespace PointOfSaleSystem.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category mappings
            CreateMap<Category, CategoryViewDto>();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();

            CreateMap<Product, ProductViewDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit!.Name));

            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductUpdateDto>();

            CreateMap<Unit, UnitViewDto>().ReverseMap();
            CreateMap<Unit, UnitCreateDto>().ReverseMap();
            CreateMap<Unit, UnitUpdateDto>().ReverseMap();

        }
    }
}
