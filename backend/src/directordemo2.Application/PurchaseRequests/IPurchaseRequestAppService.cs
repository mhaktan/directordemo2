using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.StateMachine.Dto;
using directordemo2.PurchaseRequests.Dto;

namespace directordemo2.PurchaseRequests
{
    public interface IPurchaseRequestAppService : IAsyncCrudAppService<
        PurchaseRequestDto,
        long,
        PagedPurchaseRequestResultRequestDto,
        CreatePurchaseRequestDto,
        PurchaseRequestDto>
    {
        Task<PurchaseRequestDto> ChangeStatusAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(PurchaseRequestGroupedCountInput input);
        decimal? GetStats(PurchaseRequestStatsInput input);
        Task<PurchaseRequestReportDto> GetReportData(long id);
    }
}
