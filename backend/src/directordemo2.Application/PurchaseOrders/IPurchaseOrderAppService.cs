using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.StateMachine.Dto;
using directordemo2.PurchaseOrders.Dto;

namespace directordemo2.PurchaseOrders
{
    public interface IPurchaseOrderAppService : IAsyncCrudAppService<
        PurchaseOrderDto,
        long,
        PagedPurchaseOrderResultRequestDto,
        CreatePurchaseOrderDto,
        PurchaseOrderDto>
    {
        Task<PurchaseOrderDto> ChangeStatusAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(PurchaseOrderGroupedCountInput input);
        decimal? GetStats(PurchaseOrderStatsInput input);
    }
}
