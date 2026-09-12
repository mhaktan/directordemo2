using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace directordemo2.Entities
{
    [Table("Suppliers")]
    public class Supplier : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string TaxNumber { get; set; }

        [MaxLength(200)]
        public string ContactPerson { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Quotation> Quotations { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }

    }
}