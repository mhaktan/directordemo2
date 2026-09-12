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
using directordemo2.Departments.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.Employees.Dto;
using directordemo2.PurchaseRequests.Dto;
using directordemo2.Approvals.Dto;
using directordemo2.Authorization;
using directordemo2.Flows;

namespace directordemo2.Departments
{
    public class DepartmentAppService : AsyncCrudAppService<
        Department,
        DepartmentDto,
        long,
        PagedDepartmentResultRequestDto,
        CreateDepartmentDto,
        DepartmentDto>,
        IDepartmentAppService
    {
        private readonly IFlowEngine _flowEngine;

        public DepartmentAppService(IRepository<Department, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Department_Read;
            GetAllPermissionName = PermissionNames.Department_Read;
            CreatePermissionName = PermissionNames.Department_Create;
            UpdatePermissionName = PermissionNames.Department_Update;
            DeletePermissionName = PermissionNames.Department_Delete;
        }

        protected override IQueryable<Department> CreateFilteredQuery(PagedDepartmentResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Code != null && x.Code.Contains(input.Keyword)) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), x => x.Code != null && x.Code.Contains(input.Code))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(input.AnnualBudget.HasValue, x => x.AnnualBudget == input.AnnualBudget.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value)
                .WhereIf(input.AnnualBudgetFrom.HasValue, x => x.AnnualBudget >= input.AnnualBudgetFrom.Value)
                .WhereIf(input.AnnualBudgetTo.HasValue, x => x.AnnualBudget <= input.AnnualBudgetTo.Value);
        }

        public override async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Department", result);
            return result;
        }

        public override async Task<DepartmentDto> UpdateAsync(DepartmentDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Department", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Department", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Department_Read)]
        public decimal? GetStats(DepartmentStatsInput input)
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

            var allowedNumeric = new[] { "AnnualBudget" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "AnnualBudget": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.AnnualBudget)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.AnnualBudget)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.AnnualBudget)
                            : query.Average(x => (decimal?)x.AnnualBudget);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Department_Read)]
        public async Task<DepartmentReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Employees)
                .Include(x => x.PurchaseRequests)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new DepartmentReportDto
            {
                Data = ObjectMapper.Map<DepartmentDto>(root),
                Employees = ObjectMapper.Map<List<EmployeeDto>>(
                    root.Employees == null ? new List<Employee>() : root.Employees.ToList()),
                PurchaseRequests = ObjectMapper.Map<List<PurchaseRequestDto>>(
                    root.PurchaseRequests == null ? new List<PurchaseRequest>() : root.PurchaseRequests.ToList()),
            };
        }

    }
}
