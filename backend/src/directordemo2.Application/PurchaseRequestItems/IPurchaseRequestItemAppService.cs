using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.PurchaseRequestItems.Dto;

namespace directordemo2.PurchaseRequestItems
{
    public interface IPurchaseRequestItemAppService : IAsyncCrudAppService<
        PurchaseRequestItemDto,
        long,
        PagedPurchaseRequestItemResultRequestDto,
        CreatePurchaseRequestItemDto,
        PurchaseRequestItemDto>
    {
        List<GroupCountDto> GetGroupedCount(PurchaseRequestItemGroupedCountInput input);
        decimal? GetStats(PurchaseRequestItemStatsInput input);
    }
}
