using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace directordemo2.Entities
{
    [Table("PurchaseRequestItems")]
    public class PurchaseRequestItem : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(300)]
        public string ProductName { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? LineTotal { get; set; }

        public long PurchaseRequestId { get; set; }

        [ForeignKey(nameof(PurchaseRequestId))]
        public virtual PurchaseRequest PurchaseRequest { get; set; }

    }
}