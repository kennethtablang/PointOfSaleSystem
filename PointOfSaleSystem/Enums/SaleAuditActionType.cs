namespace PointOfSaleSystem.Enums
{
    public enum SaleAuditActionType
    {
        Created = 0,
        Updated = 1,
        Voided = 2,
        VoidApproved = 3,
        DiscountApplied = 4,
        DiscountRemoved = 5,
        ItemReturned = 6,
        FullRefund = 7,
        PartialRefund = 8,
        PaymentAdded = 9,
        PaymentRemoved = 10,
        VoidRejected = 11,
        Other = 99
    }
}
