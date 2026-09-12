using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace directordemo2.Quotations.Dto
{
    [AutoMapTo(typeof(Entities.Quotation))]
    public class CreateQuotationDto
    {
        public DateTime QuotationDate { get; set; }

        public DateTime? ValidUntil { get; set; }

        public decimal Amount { get; set; }

        public bool IsSelected { get; set; }

        public long PurchaseRequestId { get; set; }

        public long SupplierId { get; set; }

    }
}