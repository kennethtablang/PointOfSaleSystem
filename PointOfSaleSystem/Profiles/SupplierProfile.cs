using AutoMapper;
using PointOfSaleSystem.DTOs.Inventory;
using PointOfSaleSystem.DTOs.Suppliers;
using PointOfSaleSystem.Models.Inventory;
using PointOfSaleSystem.Models.Suppliers;

namespace PointOfSaleSystem.Profiles
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            // Supplier
            CreateMap<Supplier, SupplierReadDto>();
            CreateMap<SupplierCreateDto, Supplier>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
            CreateMap<SupplierUpdateDto, Supplier>();

            // PurchaseOrder -> PurchaseOrderReadDto
            CreateMap<PurchaseOrder, PurchaseOrderReadDto>()
                .ForMember(d => d.SupplierName,
                    o => o.MapFrom(s => s.Supplier != null ? s.Supplier.Name : string.Empty))
                .ForMember(d => d.Items,
                    o => o.MapFrom(s => s.Items))
                // map received stocks collection if present on the model
                .ForMember(d => d.ReceivedStocks,
                    o => o.MapFrom(s => s.ReceivedStocks))
                // map status and created by username if available
                .ForMember(d => d.Status,
                    o => o.MapFrom(s => s.Status))
                .ForMember(d => d.CreatedByUserName,
                    o => o.MapFrom(s => s.CreatedByUser != null ? s.CreatedByUser.UserName : null));

            // PurchaseOrderItem -> PurchaseOrderItemReadDto
            CreateMap<PurchaseOrderItem, PurchaseOrderItemReadDto>()
                .ForMember(d => d.ProductName,
                    o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
                .ForMember(d => d.UnitName,
                    o => o.MapFrom(s => s.Unit != null ? s.Unit.Name : string.Empty))
                .ForMember(d => d.RemainingOrdered,
                    o => o.MapFrom(s => s.QuantityOrdered - s.QuantityReceived))
                .ForMember(d => d.IsClosed,
                    o => o.MapFrom(s => s.QuantityReceived >= s.QuantityOrdered));

            // ReceivedStock -> ReceivedStockReadDto
            CreateMap<ReceivedStock, ReceivedStockReadDto>()
                // product info derived from the PurchaseOrderItem navigation (if available)
                .ForMember(d => d.ProductId,
                    o => o.MapFrom(s => s.PurchaseOrderItem != null ? s.PurchaseOrderItem.ProductId : 0))
                .ForMember(d => d.ProductName,
                    o => o.MapFrom(s => s.PurchaseOrderItem != null && s.PurchaseOrderItem.Product != null
                        ? s.PurchaseOrderItem.Product.Name
                        : string.Empty))
                // map received-by username when the navigation is available; otherwise null
                .ForMember(d => d.ReceivedByUserName,
                    o => o.MapFrom(s => s.ReceivedByUser != null ? s.ReceivedByUser.UserName : null))
                .ForMember(d => d.Processed, o => o.MapFrom(s => s.Processed));

            // Create/Update mappings (leave item mapping to manual handling as before)
            CreateMap<PurchaseOrderCreateDto, PurchaseOrder>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.Items, opt => opt.Ignore()); // map items manually

            CreateMap<PurchaseOrderItemCreateDto, PurchaseOrderItem>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.QuantityReceived, opt => opt.MapFrom(_ => 0m));

            CreateMap<PurchaseOrderUpdateDto, PurchaseOrder>()
                .ForMember(d => d.Items, opt => opt.Ignore());

            CreateMap<PurchaseOrderItemUpdateDto, PurchaseOrderItem>()
                .ForMember(d => d.Id, opt => opt.Condition(src => src.Id.HasValue));
        }
    }
}
