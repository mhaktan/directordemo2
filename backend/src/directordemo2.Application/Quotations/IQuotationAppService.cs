using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.Quotations.Dto;

namespace directordemo2.Quotations
{
    public interface IQuotationAppService : IAsyncCrudAppService<
        QuotationDto,
        long,
        PagedQuotationResultRequestDto,
        CreateQuotationDto,
        QuotationDto>
    {
        List<GroupCountDto> GetGroupedCount(QuotationGroupedCountInput input);
        decimal? GetStats(QuotationStatsInput input);
    }
}
