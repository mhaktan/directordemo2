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
using directordemo2.Suppliers.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.Quotations.Dto;
using directordemo2.PurchaseOrders.Dto;
using directordemo2.Approvals.Dto;
using directordemo2.Authorization;
using directordemo2.Flows;

namespace directordemo2.Suppliers
{
    public class SupplierAppService : AsyncCrudAppService<
        Supplier,
        SupplierDto,
        long,
        PagedSupplierResultRequestDto,
        CreateSupplierDto,
        SupplierDto>,
        ISupplierAppService
    {
        private readonly IFlowEngine _flowEngine;

        public SupplierAppService(IRepository<Supplier, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Supplier_Read;
            GetAllPermissionName = PermissionNames.Supplier_Read;
            CreatePermissionName = PermissionNames.Supplier_Create;
            UpdatePermissionName = PermissionNames.Supplier_Update;
            DeletePermissionName = PermissionNames.Supplier_Delete;
        }

        protected override IQueryable<Supplier> CreateFilteredQuery(PagedSupplierResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Code != null && x.Code.Contains(input.Keyword)) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.TaxNumber != null && x.TaxNumber.Contains(input.Keyword)) ||
                    (x.ContactPerson != null && x.ContactPerson.Contains(input.Keyword)) ||
                    (x.Email != null && x.Email.Contains(input.Keyword)))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), x => x.Code != null && x.Code.Contains(input.Code))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.TaxNumber.IsNullOrWhiteSpace(), x => x.TaxNumber != null && x.TaxNumber.Contains(input.TaxNumber))
                .WhereIf(!input.ContactPerson.IsNullOrWhiteSpace(), x => x.ContactPerson != null && x.ContactPerson.Contains(input.ContactPerson))
                .WhereIf(!input.Email.IsNullOrWhiteSpace(), x => x.Email != null && x.Email.Contains(input.Email))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
        }

        public override async Task<SupplierDto> CreateAsync(CreateSupplierDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Supplier", result);
            return result;
        }

        public override async Task<SupplierDto> UpdateAsync(SupplierDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Supplier", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Supplier", new { Id = input.Id });
        }
        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Supplier_Read)]
        public async Task<SupplierReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Quotations)
                .Include(x => x.PurchaseOrders)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new SupplierReportDto
            {
                Data = ObjectMapper.Map<SupplierDto>(root),
                Quotations = ObjectMapper.Map<List<QuotationDto>>(
                    root.Quotations == null ? new List<Quotation>() : root.Quotations.ToList()),
                PurchaseOrders = ObjectMapper.Map<List<PurchaseOrderDto>>(
                    root.PurchaseOrders == null ? new List<PurchaseOrder>() : root.PurchaseOrders.ToList()),
            };
        }

    }
}
