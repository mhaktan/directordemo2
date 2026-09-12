using System;
using Abp.Application.Services.Dto;

namespace directordemo2.PurchaseRequestItems.Dto
{
    public class PagedPurchaseRequestItemResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? PurchaseRequestId { get; set; }
        public string ProductName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? LineTotal { get; set; }
        public decimal? QuantityFrom { get; set; }
        public decimal? QuantityTo { get; set; }
        public decimal? UnitPriceFrom { get; set; }
        public decimal? UnitPriceTo { get; set; }
        public decimal? LineTotalFrom { get; set; }
        public decimal? LineTotalTo { get; set; }
    }
}
