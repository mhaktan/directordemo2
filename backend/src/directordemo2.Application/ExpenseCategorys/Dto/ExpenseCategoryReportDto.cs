using System;
using System.Collections.Generic;
using directordemo2.PurchaseRequests.Dto;

namespace directordemo2.ExpenseCategorys.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class ExpenseCategoryReportDto
    {
        public ExpenseCategoryDto Data { get; set; }
        public List<PurchaseRequestDto> PurchaseRequests { get; set; }
    }
}
