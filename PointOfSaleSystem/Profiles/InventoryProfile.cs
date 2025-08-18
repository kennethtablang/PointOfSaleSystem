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

            // InventoryTransaction -> DTO
            CreateMap<InventoryTransaction, InventoryTransactionReadDto>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.PerformedByUserName,
                           opt => opt.MapFrom(src => src.PerformedByUser != null ? src.PerformedByUser.UserName : string.Empty));

            // Create DTO -> InventoryTransaction (for creating new transactions)
            CreateMap<InventoryTransactionCreateDto, InventoryTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedById, opt => opt.Ignore()) // set server-side
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate ?? DateTime.Now));

            // Update mapping if you support editing transactions
            CreateMap<InventoryTransactionUpdateDto, InventoryTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.ActionType, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedById, opt => opt.Ignore());

            // StockReceiveItem -> StockReceiveItemReadDto
            CreateMap<StockReceiveItem, StockReceiveItemReadDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.FromUnitName, opt => opt.MapFrom(src => src.FromUnit != null ? src.FromUnit.Name : null))
                .ForMember(dest => dest.ReceivedDate, opt => opt.MapFrom(src => src.StockReceive != null ? src.StockReceive.ReceivedDate : src.StockReceive!.ReceivedDate))
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.MapFrom(src => src.StockReceive != null ? src.StockReceive.PurchaseOrderId : 0))
                .ForMember(dest => dest.ReceivedByUserId, opt => opt.MapFrom(src => src.StockReceive != null ? src.StockReceive.ReceivedByUserId : string.Empty))
                .ForMember(dest => dest.ReceivedByUserName, opt => opt.MapFrom(src => src.StockReceive != null && src.StockReceive.ReceivedByUser != null ? src.StockReceive.ReceivedByUser.FullName : null));

            // StockReceive -> StockReceiveReadDto (header with items)
            CreateMap<StockReceive, StockReceiveReadDto>()
                .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.MapFrom(src => src.PurchaseOrder != null ? src.PurchaseOrder.PurchaseOrderNumber : string.Empty)) // adjust field name
                .ForMember(dest => dest.ReceivedByUserName, opt => opt.MapFrom(src => src.ReceivedByUser != null ? src.ReceivedByUser.FullName : null))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // Create DTO -> StockReceiveItem (we still compute canonical Quantity in service)
            CreateMap<StockReceiveItemCreateDto, StockReceiveItem>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.ConvertedQuantity ?? src.QuantityInFromUnit))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StockReceive, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryTransaction, opt => opt.Ignore());

            // Create → Entity
            CreateMap<StockAdjustmentCreateDto, StockAdjustment>();

            // Update → Entity
            CreateMap<StockAdjustmentUpdateDto, StockAdjustment>();

            // Entity → Read
            CreateMap<StockAdjustment, StockAdjustmentReadDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit != null ? src.Unit.Name : string.Empty))
                .ForMember(dest => dest.AdjustedByUserName, opt => opt.MapFrom(src => src.AdjustedByUser != null ? src.AdjustedByUser.FullName : string.Empty));

            // Entity → List
            CreateMap<StockAdjustment, StockAdjustmentListDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit != null ? src.Unit.Name : string.Empty))
                .ForMember(dest => dest.AdjustedByUserName, opt => opt.MapFrom(src => src.AdjustedByUser != null ? src.AdjustedByUser.FullName : string.Empty));

            // Create → Model
            CreateMap<BadOrderCreateDto, BadOrder>();

            // Update → Model
            CreateMap<BadOrderUpdateDto, BadOrder>();

            // Model → Read
            CreateMap<BadOrder, BadOrderReadDto>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ReportedByUserName,
                           opt => opt.MapFrom(src => src.ReportedByUser != null ? src.ReportedByUser.FullName : string.Empty))
                .ForMember(dest => dest.InventoryTransactionId,
                           opt => opt.MapFrom(src => src.InventoryTransactionId))
                .ForMember(dest => dest.IsSystemGenerated,
                           opt => opt.MapFrom(src => src.IsSystemGenerated));
        }
    }
}
