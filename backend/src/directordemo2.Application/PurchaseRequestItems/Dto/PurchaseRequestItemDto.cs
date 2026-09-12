using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace directordemo2.PurchaseRequestItems.Dto
{
    [AutoMapFrom(typeof(Entities.PurchaseRequestItem))]
    public class PurchaseRequestItemDto : EntityDto<long>
    {
        public string ProductName { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? LineTotal { get; set; }

        public long PurchaseRequestId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}