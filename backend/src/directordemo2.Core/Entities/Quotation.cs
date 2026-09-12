using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace directordemo2.Entities
{
    [Table("Quotations")]
    public class Quotation : FullAuditedEntity<long>
    {
        public DateTime QuotationDate { get; set; }

        public DateTime? ValidUntil { get; set; }

        public decimal Amount { get; set; }

        public bool IsSelected { get; set; }

        public long PurchaseRequestId { get; set; }

        [ForeignKey(nameof(PurchaseRequestId))]
        public virtual PurchaseRequest PurchaseRequest { get; set; }

        public long SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier Supplier { get; set; }

    }
}