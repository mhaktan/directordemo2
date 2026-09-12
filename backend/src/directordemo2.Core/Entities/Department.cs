using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace directordemo2.Entities
{
    [Table("Departments")]
    public class Department : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public decimal? AnnualBudget { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual ICollection<PurchaseRequest> PurchaseRequests { get; set; }

    }
}