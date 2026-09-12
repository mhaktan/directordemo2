using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace directordemo2.PurchaseRequests.Dto
{
    [AutoMapTo(typeof(Entities.PurchaseRequest))]
    public class CreatePurchaseRequestDto
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

        public int Status { get; set; }

        public long EmployeeId { get; set; }

        public long DepartmentId { get; set; }

        public long ExpenseCategoryId { get; set; }

    }
}