using System;
using System.Collections.Generic;
using directordemo2.PurchaseRequestItems.Dto;
using directordemo2.Quotations.Dto;
using directordemo2.PurchaseOrders.Dto;
using directordemo2.Approvals.Dto;

namespace directordemo2.PurchaseRequests.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class PurchaseRequestReportDto
    {
        public PurchaseRequestDto Data { get; set; }
        public List<PurchaseRequestItemDto> PurchaseRequestItems { get; set; }
        public List<QuotationDto> Quotations { get; set; }
        public List<PurchaseOrderDto> PurchaseOrders { get; set; }
        public List<ApprovalRecordDto> ApprovalHistory { get; set; }
    }
}
