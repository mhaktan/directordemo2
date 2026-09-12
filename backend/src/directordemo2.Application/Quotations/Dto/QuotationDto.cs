using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace directordemo2.Quotations.Dto
{
    [AutoMapFrom(typeof(Entities.Quotation))]
    public class QuotationDto : EntityDto<long>
    {
        public DateTime QuotationDate { get; set; }

        public DateTime? ValidUntil { get; set; }

        public decimal Amount { get; set; }

        public bool IsSelected { get; set; }

        public long PurchaseRequestId { get; set; }

        public long SupplierId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}