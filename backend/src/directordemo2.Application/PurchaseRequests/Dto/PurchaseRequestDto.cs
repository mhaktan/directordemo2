using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace directordemo2.PurchaseRequests.Dto
{
    [AutoMapFrom(typeof(Entities.PurchaseRequest))]
    public class PurchaseRequestDto : EntityDto<long>
    {
        public string RequestNumber { get; set; }

        public string Justification { get; set; }

        public decimal? TotalAmount { get; set; }

        public DateTime NeededDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string RejectionReason { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long EmployeeId { get; set; }

        public long DepartmentId { get; set; }

        public long ExpenseCategoryId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}