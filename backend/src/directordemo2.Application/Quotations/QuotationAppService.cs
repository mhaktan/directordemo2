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
using directordemo2.Quotations.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.Authorization;
using directordemo2.Flows;

namespace directordemo2.Quotations
{
    public class QuotationAppService : AsyncCrudAppService<
        Quotation,
        QuotationDto,
        long,
        PagedQuotationResultRequestDto,
        CreateQuotationDto,
        QuotationDto>,
        IQuotationAppService
    {
        private readonly IFlowEngine _flowEngine;

        public QuotationAppService(IRepository<Quotation, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Quotation_Read;
            GetAllPermissionName = PermissionNames.Quotation_Read;
            CreatePermissionName = PermissionNames.Quotation_Create;
            UpdatePermissionName = PermissionNames.Quotation_Update;
            DeletePermissionName = PermissionNames.Quotation_Delete;
        }

        protected override IQueryable<Quotation> CreateFilteredQuery(PagedQuotationResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword))
                .WhereIf(input.QuotationDate.HasValue, x => x.QuotationDate == input.QuotationDate.Value)
                .WhereIf(input.ValidUntil.HasValue, x => x.ValidUntil == input.ValidUntil.Value)
                .WhereIf(input.Amount.HasValue, x => x.Amount == input.Amount.Value)
                .WhereIf(input.IsSelected.HasValue, x => x.IsSelected == input.IsSelected.Value)
                .WhereIf(input.QuotationDateFrom.HasValue, x => x.QuotationDate >= input.QuotationDateFrom.Value)
                .WhereIf(input.QuotationDateTo.HasValue, x => x.QuotationDate <= input.QuotationDateTo.Value)
                .WhereIf(input.ValidUntilFrom.HasValue, x => x.ValidUntil >= input.ValidUntilFrom.Value)
                .WhereIf(input.ValidUntilTo.HasValue, x => x.ValidUntil <= input.ValidUntilTo.Value)
                .WhereIf(input.AmountFrom.HasValue, x => x.Amount >= input.AmountFrom.Value)
                .WhereIf(input.AmountTo.HasValue, x => x.Amount <= input.AmountTo.Value)
                .WhereIf(input.PurchaseRequestId.HasValue, x => x.PurchaseRequestId == input.PurchaseRequestId.Value)
                .WhereIf(input.SupplierId.HasValue, x => x.SupplierId == input.SupplierId.Value);
        }

        public override async Task<QuotationDto> CreateAsync(CreateQuotationDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Quotation", result);
            return result;
        }

        public override async Task<QuotationDto> UpdateAsync(QuotationDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Quotation", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Quotation", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Quotation_Read)]
        public List<GroupCountDto> GetGroupedCount(QuotationGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "PurchaseRequestId", "SupplierId" };
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
                case "SupplierId":
                    return query
                        .GroupBy(x => new { Key = x.SupplierId, Label = x.Supplier == null ? null : x.Supplier.Name })
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.Quotation_Read)]
        public decimal? GetStats(QuotationStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "QuotationDate", "ValidUntil" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "QuotationDate|ValidUntil":
                    {
                        var pairsQuotationDateValidUntil = query
                            .Where(x => x.ValidUntil != null)
                            .Select(x => new { A = x.QuotationDate, B = x.ValidUntil.Value })
                            .ToList();
                        if (pairsQuotationDateValidUntil.Count == 0) return null;
                        return (decimal)pairsQuotationDateValidUntil.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "ValidUntil|QuotationDate":
                    {
                        var pairsValidUntilQuotationDate = query
                            .Where(x => x.ValidUntil != null)
                            .Select(x => new { A = x.ValidUntil.Value, B = x.QuotationDate })
                            .ToList();
                        if (pairsValidUntilQuotationDate.Count == 0) return null;
                        return (decimal)pairsValidUntilQuotationDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Amount" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Amount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Amount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Amount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Amount)
                            : query.Average(x => (decimal?)x.Amount);
                        default: return null;
            }
        }

    }
}
