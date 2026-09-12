using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace directordemo2.Entities
{
    // State Machine: status — Draft → PendingApproval → Approved → Shipped → Delivered → Cancelled
    // Initial: Draft | Transitions: Draft→PendingApproval[Submit], PendingApproval→Approved[Approve], PendingApproval→Draft[Revise], Approved→Shipped[MarkShipped], Shipped→Delivered[MarkDelivered], *→Cancelled[Cancel]
    [Table("PurchaseOrders")]
    public class PurchaseOrder : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public decimal OrderAmount { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public long PurchaseRequestId { get; set; }

        [ForeignKey(nameof(PurchaseRequestId))]
        public virtual PurchaseRequest PurchaseRequest { get; set; }

        public long SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier Supplier { get; set; }

    }
}