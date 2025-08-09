using AutoMapper;
using PointOfSaleSystem.DTOs.Suppliers;
using PointOfSaleSystem.Models.Suppliers;

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

            // PurchaseOrder
            CreateMap<PurchaseOrder, PurchaseOrderReadDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty))
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.UserName : string.Empty));

            CreateMap<PurchaseOrderCreateDto, PurchaseOrder>();
            CreateMap<PurchaseOrderUpdateDto, PurchaseOrder>();

            // PurchaseItem
            CreateMap<PurchaseItem, PurchaseItemReadDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));

            CreateMap<PurchaseItemCreateDto, PurchaseItem>();
            CreateMap<PurchaseItemUpdateDto, PurchaseItem>();

            // ReceivedStock
            CreateMap<ReceivedStock, ReceivedStockReadDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ReceivedByUserName, opt => opt.MapFrom(src => src.ReceivedByUser != null ? src.ReceivedByUser.UserName : string.Empty));

            CreateMap<ReceivedStockCreateDto, ReceivedStock>();
        }
    }
}
