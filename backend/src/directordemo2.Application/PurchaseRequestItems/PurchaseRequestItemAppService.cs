using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using directordemo2.Entities;
using directordemo2.PurchaseRequestItems.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.Authorization;
using directordemo2.Flows;

namespace directordemo2.PurchaseRequestItems
{
    public class PurchaseRequestItemAppService : AsyncCrudAppService<
        PurchaseRequestItem,
        PurchaseRequestItemDto,
        long,
        PagedPurchaseRequestItemResultRequestDto,
        CreatePurchaseRequestItemDto,
        PurchaseRequestItemDto>,
        IPurchaseRequestItemAppService
    {
        private readonly IFlowEngine _flowEngine;

        public PurchaseRequestItemAppService(IRepository<PurchaseRequestItem, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.PurchaseRequestItem_Read;
            GetAllPermissionName = PermissionNames.PurchaseRequestItem_Read;
            CreatePermissionName = PermissionNames.PurchaseRequestItem_Create;
            UpdatePermissionName = PermissionNames.PurchaseRequestItem_Update;
            DeletePermissionName = PermissionNames.PurchaseRequestItem_Delete;
        }

        protected override IQueryable<PurchaseRequestItem> CreateFilteredQuery(PagedPurchaseRequestItemResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.ProductName != null && x.ProductName.Contains(input.Keyword)))
                .WhereIf(!input.ProductName.IsNullOrWhiteSpace(), x => x.ProductName != null && x.ProductName.Contains(input.ProductName))
                .WhereIf(input.Quantity.HasValue, x => x.Quantity == input.Quantity.Value)
                .WhereIf(input.UnitPrice.HasValue, x => x.UnitPrice == input.UnitPrice.Value)
                .WhereIf(input.LineTotal.HasValue, x => x.LineTotal == input.LineTotal.Value)
                .WhereIf(input.QuantityFrom.HasValue, x => x.Quantity >= input.QuantityFrom.Value)
                .WhereIf(input.QuantityTo.HasValue, x => x.Quantity <= input.QuantityTo.Value)
                .WhereIf(input.UnitPriceFrom.HasValue, x => x.UnitPrice >= input.UnitPriceFrom.Value)
                .WhereIf(input.UnitPriceTo.HasValue, x => x.UnitPrice <= input.UnitPriceTo.Value)
                .WhereIf(input.LineTotalFrom.HasValue, x => x.LineTotal >= input.LineTotalFrom.Value)
                .WhereIf(input.LineTotalTo.HasValue, x => x.LineTotal <= input.LineTotalTo.Value)
                .WhereIf(input.PurchaseRequestId.HasValue, x => x.PurchaseRequestId == input.PurchaseRequestId.Value);
        }

        public override async Task<PurchaseRequestItemDto> CreateAsync(CreatePurchaseRequestItemDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "PurchaseRequestItem", result);
            return result;
        }

        public override async Task<PurchaseRequestItemDto> UpdateAsync(PurchaseRequestItemDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "PurchaseRequestItem", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "PurchaseRequestItem", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.PurchaseRequestItem_Read)]
        public List<GroupCountDto> GetGroupedCount(PurchaseRequestItemGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "PurchaseRequestId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "PurchaseRequestId":
                    return query
                        .GroupBy(x => new { Key = x.PurchaseRequestId, Label = x.PurchaseRequest == null ? null : x.PurchaseRequest.RequestNumber })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.PurchaseRequestItem_Read)]
        public decimal? GetStats(PurchaseRequestItemStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new string[0];
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Quantity", "UnitPrice", "LineTotal" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Quantity": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Quantity)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Quantity)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Quantity)
                            : query.Average(x => (decimal?)x.Quantity);
                        case "UnitPrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.UnitPrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.UnitPrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.UnitPrice)
                            : query.Average(x => (decimal?)x.UnitPrice);
                        case "LineTotal": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.LineTotal)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.LineTotal)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.LineTotal)
                            : query.Average(x => (decimal?)x.LineTotal);
                        default: return null;
            }
        }

    }
}
