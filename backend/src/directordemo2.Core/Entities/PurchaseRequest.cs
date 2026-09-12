using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace directordemo2.Entities
{
    // State Machine: status — Draft → PendingManagerApproval → PendingFinanceApproval → Approved → Ordered
    // Initial: Draft | Transitions: Draft→PendingManagerApproval[Submit], PendingManagerApproval→PendingFinanceApproval[Approve], PendingManagerApproval→Draft[Revise], PendingFinanceApproval→Approved[Approve], PendingFinanceApproval→Draft[Revise], Approved→Ordered[PlaceOrder]
    [Table("PurchaseRequests")]
    public class PurchaseRequest : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(50)]
        public string RequestNumber { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Justification { get; set; }

        public decimal? TotalAmount { get; set; }

        public DateTime NeededDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [MaxLength(1000)]
        public string RejectionReason { get; set; }

        public PurchaseRequestStatus Status { get; set; }

        public long EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }

        public long ExpenseCategoryId { get; set; }

        [ForeignKey(nameof(ExpenseCategoryId))]
        public virtual ExpenseCategory ExpenseCategory { get; set; }

        public virtual ICollection<PurchaseRequestItem> PurchaseRequestItems { get; set; }

        public virtual ICollection<Quotation> Quotations { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }

    }
}