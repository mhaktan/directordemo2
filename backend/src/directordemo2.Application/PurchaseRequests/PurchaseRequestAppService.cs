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
using directordemo2.PurchaseRequests.Dto;
using directordemo2.Analytics.Dto;
using directordemo2.StateMachine.Dto;
using directordemo2.PurchaseRequestItems.Dto;
using directordemo2.Quotations.Dto;
using directordemo2.PurchaseOrders.Dto;
using directordemo2.Approvals.Dto;
using directordemo2.Authorization;
using directordemo2.Flows;

namespace directordemo2.PurchaseRequests
{
    public class PurchaseRequestAppService : AsyncCrudAppService<
        PurchaseRequest,
        PurchaseRequestDto,
        long,
        PagedPurchaseRequestResultRequestDto,
        CreatePurchaseRequestDto,
        PurchaseRequestDto>,
        IPurchaseRequestAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public PurchaseRequestAppService(IRepository<PurchaseRequest, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.PurchaseRequest_Read;
            GetAllPermissionName = PermissionNames.PurchaseRequest_Read;
            CreatePermissionName = PermissionNames.PurchaseRequest_Create;
            UpdatePermissionName = PermissionNames.PurchaseRequest_Update;
            DeletePermissionName = PermissionNames.PurchaseRequest_Delete;
        }

        protected override IQueryable<PurchaseRequest> CreateFilteredQuery(PagedPurchaseRequestResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.RequestNumber != null && x.RequestNumber.Contains(input.Keyword)) ||
                    (x.Justification != null && x.Justification.Contains(input.Keyword)) ||
                    (x.RejectionReason != null && x.RejectionReason.Contains(input.Keyword)))
                .WhereIf(!input.RequestNumber.IsNullOrWhiteSpace(), x => x.RequestNumber != null && x.RequestNumber.Contains(input.RequestNumber))
                .WhereIf(!input.Justification.IsNullOrWhiteSpace(), x => x.Justification != null && x.Justification.Contains(input.Justification))
                .WhereIf(!input.RejectionReason.IsNullOrWhiteSpace(), x => x.RejectionReason != null && x.RejectionReason.Contains(input.RejectionReason))
                .WhereIf(input.TotalAmount.HasValue, x => x.TotalAmount == input.TotalAmount.Value)
                .WhereIf(input.NeededDate.HasValue, x => x.NeededDate == input.NeededDate.Value)
                .WhereIf(input.ApprovedDate.HasValue, x => x.ApprovedDate == input.ApprovedDate.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (PurchaseRequestStatus)input.Status.Value)
                .WhereIf(input.TotalAmountFrom.HasValue, x => x.TotalAmount >= input.TotalAmountFrom.Value)
                .WhereIf(input.TotalAmountTo.HasValue, x => x.TotalAmount <= input.TotalAmountTo.Value)
                .WhereIf(input.NeededDateFrom.HasValue, x => x.NeededDate >= input.NeededDateFrom.Value)
                .WhereIf(input.NeededDateTo.HasValue, x => x.NeededDate <= input.NeededDateTo.Value)
                .WhereIf(input.ApprovedDateFrom.HasValue, x => x.ApprovedDate >= input.ApprovedDateFrom.Value)
                .WhereIf(input.ApprovedDateTo.HasValue, x => x.ApprovedDate <= input.ApprovedDateTo.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (PurchaseRequestStatus)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (PurchaseRequestStatus)input.StatusNot.Value)
                .WhereIf(input.EmployeeId.HasValue, x => x.EmployeeId == input.EmployeeId.Value)
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId == input.DepartmentId.Value)
                .WhereIf(input.ExpenseCategoryId.HasValue, x => x.ExpenseCategoryId == input.ExpenseCategoryId.Value);
        }

        public override async Task<PurchaseRequestDto> CreateAsync(CreatePurchaseRequestDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "PurchaseRequest", result);

            // Frontend creates records with status pre-set without going through ChangeStatusAsync,
            // so mirror on-field-change here whenever the initial status isn't the default. Otherwise
            // status-driven flows (e.g. approval) never fire on plain Create.
            if (result.Status != (int)PurchaseRequestStatus.Draft)
                await _flowEngine.TriggerAsync("on-field-change", "PurchaseRequest", result);
            return result;
        }

        public override async Task<PurchaseRequestDto> UpdateAsync(PurchaseRequestDto input)
        {
            // State machine: validate status transition + log
            var existing = await Repository.GetAsync(input.Id);
            var statusChanged = (int)existing.Status != input.Status;
            if (statusChanged)
            {
                var fromStatus = existing.Status.ToString();
                var toStatus = ((PurchaseRequestStatus)input.Status).ToString();
                ValidateStatusTransition(existing.Status, (PurchaseRequestStatus)input.Status);

                // Log status change
                await _statusChangeLogRepo.InsertAsync(new StatusChangeLog
                {
                    EntityType = "PurchaseRequest",
                    EntityId = input.Id.ToString(),
                    FromStatus = fromStatus,
                    ToStatus = toStatus,
                    Action = "Update",
                    ChangedByUserId = AbpSession.UserId
                });
            }

            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "PurchaseRequest", result);

            // Frontend updates status via plain UpdateAsync (not ChangeStatusAsync) — fire
            // on-field-change so status-driven flows pick up the transition.
            if (statusChanged)
                await _flowEngine.TriggerAsync("on-field-change", "PurchaseRequest", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "PurchaseRequest", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.PurchaseRequest_ChangeStatus, PermissionNames.PurchaseRequest_Update, RequireAllPermissions = false)]
        public async Task<PurchaseRequestDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Draft", "PendingManagerApproval", "Submit", false),
            ("PendingManagerApproval", "PendingFinanceApproval", "Approve", false),
            ("PendingManagerApproval", "Draft", "Revise", false),
            ("PendingFinanceApproval", "Approved", "Approve", false),
            ("PendingFinanceApproval", "Draft", "Revise", false),
            ("Approved", "Ordered", "PlaceOrder", false)
            };

            var transition = transitions.FirstOrDefault(t =>
                (t.From == "*" || t.From == currentStatus) && t.Action == input.Action);

            if (transition == default)
                throw new Abp.UI.UserFriendlyException($"Invalid action '{input.Action}' from status '{currentStatus}'");

            // Validate required fields per transition
            if (input.Action == "Revise" && (input.ActionData == null || !input.ActionData.ContainsKey("rejectionReason") || string.IsNullOrWhiteSpace(input.ActionData["rejectionReason"])))
                throw new Abp.UI.UserFriendlyException("Revise requires: rejectionReason");
            // Bu gecisler icin bagli kayit on kosulu yok

            var fromStatus = currentStatus;

            // Apply new status
            entity.Status = (PurchaseRequestStatus)Enum.Parse(typeof(PurchaseRequestStatus), transition.To);
            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "PurchaseRequest" && a.EntityId == id.ToString() && a.Status == "Pending")
                    .ToList();
                foreach (var pendingRec in pending)
                {
                    pendingRec.Status = "Cancelled";
                    pendingRec.ActionTaken = "Cancel";
                    pendingRec.ActionDate = DateTime.UtcNow;
                    pendingRec.Comment = "Entity cancelled by submitter.";
                    await _approvalRepo.UpdateAsync(pendingRec);
                }
            }

            // Log status change
            await _statusChangeLogRepo.InsertAsync(new Entities.StatusChangeLog
            {
                EntityType = "PurchaseRequest",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "PurchaseRequest", result);

            // Trigger named flow events
            if (input.Action == "Submit")
                await _flowEngine.TriggerAsync("submit-for-approval", "PurchaseRequest", result);
            return result;
        }

        private void ValidateStatusTransition(PurchaseRequestStatus from, PurchaseRequestStatus to)
        {
            var allowed = new (string From, string To)[]
            {
                ("Draft", "PendingManagerApproval"),
                ("PendingManagerApproval", "PendingFinanceApproval"),
                ("PendingManagerApproval", "Draft"),
                ("PendingFinanceApproval", "Approved"),
                ("PendingFinanceApproval", "Draft"),
                ("Approved", "Ordered")
            };

            var isValid = allowed.Any(t =>
                (t.From == "*" || t.From == from.ToString()) &&
                t.To == to.ToString());

            if (!isValid)
                throw new Abp.UI.UserFriendlyException($"Invalid status transition from {from} to {to}");
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.PurchaseRequest_Read)]
        public List<GroupCountDto> GetGroupedCount(PurchaseRequestGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Status", "EmployeeId", "DepartmentId", "ExpenseCategoryId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "Status":
                    return query
                        .GroupBy(x => x.Status)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "EmployeeId":
                    return query
                        .GroupBy(x => new { Key = x.EmployeeId, Label = x.Employee == null ? null : x.Employee.FullName })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "DepartmentId":
                    return query
                        .GroupBy(x => new { Key = x.DepartmentId, Label = x.Department == null ? null : x.Department.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "ExpenseCategoryId":
                    return query
                        .GroupBy(x => new { Key = x.ExpenseCategoryId, Label = x.ExpenseCategory == null ? null : x.ExpenseCategory.Name })
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.PurchaseRequest_Read)]
        public decimal? GetStats(PurchaseRequestStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "NeededDate", "ApprovedDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "NeededDate|ApprovedDate":
                    {
                        var pairsNeededDateApprovedDate = query
                            .Where(x => x.ApprovedDate != null)
                            .Select(x => new { A = x.NeededDate, B = x.ApprovedDate.Value })
                            .ToList();
                        if (pairsNeededDateApprovedDate.Count == 0) return null;
                        return (decimal)pairsNeededDateApprovedDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "ApprovedDate|NeededDate":
                    {
                        var pairsApprovedDateNeededDate = query
                            .Where(x => x.ApprovedDate != null)
                            .Select(x => new { A = x.ApprovedDate.Value, B = x.NeededDate })
                            .ToList();
                        if (pairsApprovedDateNeededDate.Count == 0) return null;
                        return (decimal)pairsApprovedDateNeededDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "TotalAmount" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "TotalAmount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.TotalAmount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.TotalAmount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.TotalAmount)
                            : query.Average(x => (decimal?)x.TotalAmount);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.PurchaseRequest_Read)]
        public async Task<PurchaseRequestReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.PurchaseRequestItems)
                .Include(x => x.Quotations)
                .Include(x => x.PurchaseOrders)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new PurchaseRequestReportDto
            {
                Data = ObjectMapper.Map<PurchaseRequestDto>(root),
                PurchaseRequestItems = ObjectMapper.Map<List<PurchaseRequestItemDto>>(
                    root.PurchaseRequestItems == null ? new List<PurchaseRequestItem>() : root.PurchaseRequestItems.ToList()),
                Quotations = ObjectMapper.Map<List<QuotationDto>>(
                    root.Quotations == null ? new List<Quotation>() : root.Quotations.ToList()),
                PurchaseOrders = ObjectMapper.Map<List<PurchaseOrderDto>>(
                    root.PurchaseOrders == null ? new List<PurchaseOrder>() : root.PurchaseOrders.ToList()),
                // ApprovalRecord alan adlari: EntityType (isim) ve EntityId (STRING).
                // Once EntityName/long varsayilmisti — CS1061 + CS0019 veriyordu.
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityType == "PurchaseRequest" && a.EntityId == id.ToString())
                        .OrderBy(a => a.StepIndex).ToList()),
            };
        }

    }
}
