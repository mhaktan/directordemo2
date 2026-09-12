namespace directordemo2.Entities
{
    public enum PurchaseRequestStatus
    {
        Draft = 0,
        PendingManagerApproval = 1,
        PendingFinanceApproval = 2,
        Approved = 3,
        Ordered = 4,
    }
}