using System;
using directordemo2.Analytics.Dto;

namespace directordemo2.Quotations.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class QuotationGroupedCountInput : PagedQuotationResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class QuotationStatsInput : PagedQuotationResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
