using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace directordemo2.PurchaseRequestItems.Dto
{
    [AutoMapTo(typeof(Entities.PurchaseRequestItem))]
    public class CreatePurchaseRequestItemDto
    {
        [Required]
        [MaxLength(300)]
        public string ProductName { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? LineTotal { get; set; }

        public long PurchaseRequestId { get; set; }

    }
}