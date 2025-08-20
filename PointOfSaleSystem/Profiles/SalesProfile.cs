using AutoMapper;
using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Models.Sales;

namespace PointOfSaleSystem.Profiles
{
    public class SalesProfile : Profile
    {
        public SalesProfile()
        {
            // SALE / SALE ITEM
            CreateMap<SaleCreateDto, Sale>()
                .ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.SaleDate ?? System.DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
                .ForMember(dest => dest.TotalDiscount, opt => opt.Ignore())
                .ForMember(dest => dest.VatAmount, opt => opt.Ignore())
                .ForMember(dest => dest.NonVatAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.IsFullyRefunded, opt => opt.Ignore())
                .ForMember(dest => dest.RefundedAt, opt => opt.Ignore())
                .ForMember(dest => dest.SaleItems, opt => opt.MapFrom(src => src.Items));

            CreateMap<SaleItemCreateDto, SaleItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore())
                .ForMember(dest => dest.ComputedTotal, opt => opt.Ignore())
                .ForMember(dest => dest.ReturnedQuantity, opt => opt.Ignore())
                .ForMember(dest => dest.CostPrice, opt => opt.MapFrom(src => src.CostPrice ?? 0m));

            CreateMap<Sale, SaleReadDto>()
                .ForMember(dst => dst.CashierName, opt => opt.MapFrom(src => src.Cashier != null ? src.Cashier.FullName : string.Empty))
                .ForMember(dst => dst.Items, opt => opt.MapFrom(src => src.SaleItems))
                .ForMember(dst => dst.Payments, opt => opt.MapFrom(src => src.Payments))
                .ForMember(dst => dst.Discounts, opt => opt.MapFrom(src => src.Discounts));
            // removed mapping to Returns since SaleReadDto does not have that property

            CreateMap<SaleItem, SaleItemReadDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
                .ForMember(d => d.UnitName, o => o.MapFrom(s => s.Unit != null ? s.Unit.Name : string.Empty))
                .ForMember(d => d.CostPrice, o => o.MapFrom(s => s.CostPrice));

            // PAYMENT
            CreateMap<PaymentCreateDto, Payment>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.PaymentDate, o => o.MapFrom(src => src.PaymentDate ?? System.DateTime.UtcNow))
                .ForMember(d => d.UserId, o => o.MapFrom(src => src.UserId))
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.ChangeAmount, o => o.MapFrom(src => src.ChangeAmount));

            CreateMap<Payment, PaymentReadDto>()
                .ForMember(d => d.PerformedByUserId, o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.PerformedByUserName, o => o.MapFrom(s => s.User != null ? s.User.FullName : string.Empty))
                .ForMember(d => d.ChangeAmount, o => o.MapFrom(s => s.ChangeAmount));

            // RETURN TRANSACTION & ITEMS
            CreateMap<ReturnedItemCreateDto, ReturnedItem>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.UnitPrice, o => o.MapFrom(src => src.UnitPrice));

            CreateMap<ReturnedItemUpdateDto, ReturnedItem>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<ReturnedItem, ReturnedItemReadDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
                .ForMember(d => d.TotalRefundAmount, o => o.MapFrom(s => s.UnitPrice * s.Quantity));

            CreateMap<ReturnTransactionCreateDto, ReturnTransaction>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ReturnDate, o => o.MapFrom(src => src.ReturnDate ?? System.DateTime.UtcNow))
                .ForMember(d => d.ReturnedByUserId, o => o.MapFrom(src => src.ReturnedByUserId ?? string.Empty))
                .ForMember(d => d.Items, o => o.MapFrom(src => src.Items))
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.RefundMethod, o => o.MapFrom(src => src.RefundMethod));

            CreateMap<ReturnTransactionUpdateDto, ReturnTransaction>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<ReturnTransaction, ReturnTransactionReadDto>()
                .ForMember(d => d.ReturnedByUserName, o => o.MapFrom(s => s.ReturnedBy != null ? s.ReturnedBy.FullName : string.Empty))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            // DISCOUNT
            CreateMap<DiscountCreateDto, Discount>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.AppliedAt, o => o.MapFrom(src => src.AppliedAt ?? System.DateTime.UtcNow))
                .ForMember(d => d.PercentApplied, o => o.MapFrom(src => src.PercentApplied ?? 0m));

            CreateMap<DiscountUpdateDto, Discount>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ApprovedByUserId, o => o.MapFrom(src => src.ApprovedByUserId));

            CreateMap<Discount, DiscountReadDto>()
                .ForMember(d => d.AppliedByUserName, o => o.MapFrom(s => s.AppliedByUser != null ? s.AppliedByUser.FullName : string.Empty))
                .ForMember(d => d.ApprovedByUserName, o => o.MapFrom(s => s.ApprovedByUser != null ? s.ApprovedByUser.FullName : string.Empty))
                .ForMember(d => d.DiscountSettingName, o => o.MapFrom(s => s.DiscountSetting != null ? s.DiscountSetting.Name : string.Empty));

            // VOID TRANSACTION
            CreateMap<VoidTransactionCreateDto, VoidTransaction>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.VoidedAt, o => o.MapFrom(src => src.VoidedAt ?? System.DateTime.UtcNow))
                .ForMember(d => d.IsSystemVoid, o => o.MapFrom(src => src.IsSystemVoid));

            CreateMap<VoidTransaction, VoidTransactionReadDto>()
                .ForMember(d => d.VoidedByUserName, o => o.MapFrom(s => s.VoidedBy != null ? s.VoidedBy.FullName : string.Empty))
                .ForMember(d => d.ApprovalUserName, o => o.MapFrom(s => s.ApprovalUser != null ? s.ApprovalUser.FullName : string.Empty))
                .ForMember(d => d.OriginalCashierName, o => o.MapFrom(s => s.OriginalCashier != null ? s.OriginalCashier.FullName : string.Empty));

            // SALE AUDIT TRAIL
            CreateMap<SaleAuditTrailCreateDto, SaleAuditTrail>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ActionAt, o => o.MapFrom(src => src.ActionAt ?? System.DateTime.UtcNow))
                .ForMember(d => d.PerformedByUserId, o => o.MapFrom(src => src.PerformedByUserId));

            CreateMap<SaleAuditTrail, SaleAuditTrailReadDto>()
                .ForMember(d => d.PerformedByUserName, o => o.MapFrom(s => s.PerformedBy != null ? s.PerformedBy.FullName : string.Empty));

            // ReceiptLog mappings
            CreateMap<ReceiptLogCreateDto, ReceiptLog>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.PrintedAt, o => o.MapFrom(src => src.PrintedAt ?? System.DateTime.UtcNow));

            CreateMap<ReceiptLog, ReceiptLogReadDto>()
                .ForMember(d => d.PrintedByUserName, o => o.MapFrom(s => s.PrintedBy != null ? s.PrintedBy.FullName : string.Empty));
        }
    }
}
