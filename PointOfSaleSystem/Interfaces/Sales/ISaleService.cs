using PointOfSaleSystem.DTOs.Sales;
using PointOfSaleSystem.Enums;
using PointOfSaleSystem.Models.Sales;

namespace PointOfSaleSystem.Interfaces.Sales
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleReadDto>> GetAllAsync(DateTime? from = null, DateTime? to = null, string? cashierUserId = null, string? invoiceNumber = null);
        Task<SaleReadDto?> GetByIdAsync(int id);
        Task<SaleReadDto?> GetByInvoiceNumberAsync(string invoiceNumber);
        Task<SaleReadDto> CreateSaleAsync(SaleCreateDto dto, string cashierUserId);
        Task<VoidTransactionReadDto> VoidSaleAsync(int saleId, string voidedByUserId, string reason, bool isSystemVoid = false, string? approvalUserId = null);
        Task<PaymentReadDto> AddPaymentAsync(int saleId, PaymentCreateDto dto, string performedByUserId);
        Task<SaleReadDto> FullRefundAsync(int saleId, string performedByUserId, RefundMethod refundMethod);
        Task<SaleReadDto?> UpdateSaleAsync(int saleId, SaleCreateDto dto);
        Task<IEnumerable<SaleAuditTrailReadDto>> GetAuditTrailsAsync(int saleId);
    }
}
