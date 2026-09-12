using System;
using Abp.Application.Services.Dto;

namespace directordemo2.PurchaseRequests.Dto
{
    public class PagedPurchaseRequestResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? EmployeeId { get; set; }
        public long? DepartmentId { get; set; }
        public long? ExpenseCategoryId { get; set; }
        public string RequestNumber { get; set; }
        public string Justification { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? NeededDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectionReason { get; set; }
        public int? Status { get; set; }
        public decimal? TotalAmountFrom { get; set; }
        public decimal? TotalAmountTo { get; set; }
        public DateTime? NeededDateFrom { get; set; }
        public DateTime? NeededDateTo { get; set; }
        public DateTime? ApprovedDateFrom { get; set; }
        public DateTime? ApprovedDateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
