using System;
using System.Collections.Generic;
using directordemo2.Quotations.Dto;
using directordemo2.PurchaseOrders.Dto;

namespace directordemo2.Suppliers.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class SupplierReportDto
    {
        public SupplierDto Data { get; set; }
        public List<QuotationDto> Quotations { get; set; }
        public List<PurchaseOrderDto> PurchaseOrders { get; set; }
    }
}
