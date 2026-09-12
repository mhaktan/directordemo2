using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace directordemo2.Entities
{
    [Table("Employees")]
    public class Employee : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(50)]
        public string RegistrationNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        public bool IsActive { get; set; }

        public long? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }

        public virtual ICollection<PurchaseRequest> PurchaseRequests { get; set; }

    }
}