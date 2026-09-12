using Abp.Authorization;
using Abp.Localization;

namespace directordemo2.Authorization
{
    public class directordemo2AuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull("Pages") ?? context.CreatePermission("Pages", L("Pages"));

            // Department
            pages.CreateChildPermission(PermissionNames.Department_Read, L("Department.Read"));
            pages.CreateChildPermission(PermissionNames.Department_Create, L("Department.Create"));
            pages.CreateChildPermission(PermissionNames.Department_Update, L("Department.Update"));
            pages.CreateChildPermission(PermissionNames.Department_Delete, L("Department.Delete"));

            // Employee
            pages.CreateChildPermission(PermissionNames.Employee_Read, L("Employee.Read"));
            pages.CreateChildPermission(PermissionNames.Employee_Create, L("Employee.Create"));
            pages.CreateChildPermission(PermissionNames.Employee_Update, L("Employee.Update"));
            pages.CreateChildPermission(PermissionNames.Employee_Delete, L("Employee.Delete"));

            // Supplier
            pages.CreateChildPermission(PermissionNames.Supplier_Read, L("Supplier.Read"));
            pages.CreateChildPermission(PermissionNames.Supplier_Create, L("Supplier.Create"));
            pages.CreateChildPermission(PermissionNames.Supplier_Update, L("Supplier.Update"));
            pages.CreateChildPermission(PermissionNames.Supplier_Delete, L("Supplier.Delete"));

            // ExpenseCategory
            pages.CreateChildPermission(PermissionNames.ExpenseCategory_Read, L("ExpenseCategory.Read"));
            pages.CreateChildPermission(PermissionNames.ExpenseCategory_Create, L("ExpenseCategory.Create"));
            pages.CreateChildPermission(PermissionNames.ExpenseCategory_Update, L("ExpenseCategory.Update"));
            pages.CreateChildPermission(PermissionNames.ExpenseCategory_Delete, L("ExpenseCategory.Delete"));

            // PurchaseRequest
            pages.CreateChildPermission(PermissionNames.PurchaseRequest_Read, L("PurchaseRequest.Read"));
            pages.CreateChildPermission(PermissionNames.PurchaseRequest_Create, L("PurchaseRequest.Create"));
            pages.CreateChildPermission(PermissionNames.PurchaseRequest_Update, L("PurchaseRequest.Update"));
            pages.CreateChildPermission(PermissionNames.PurchaseRequest_Delete, L("PurchaseRequest.Delete"));
            pages.CreateChildPermission(PermissionNames.PurchaseRequest_ChangeStatus, L("PurchaseRequest.ChangeStatus"));

            // PurchaseRequestItem
            pages.CreateChildPermission(PermissionNames.PurchaseRequestItem_Read, L("PurchaseRequestItem.Read"));
            pages.CreateChildPermission(PermissionNames.PurchaseRequestItem_Create, L("PurchaseRequestItem.Create"));
            pages.CreateChildPermission(PermissionNames.PurchaseRequestItem_Update, L("PurchaseRequestItem.Update"));
            pages.CreateChildPermission(PermissionNames.PurchaseRequestItem_Delete, L("PurchaseRequestItem.Delete"));

            // Quotation
            pages.CreateChildPermission(PermissionNames.Quotation_Read, L("Quotation.Read"));
            pages.CreateChildPermission(PermissionNames.Quotation_Create, L("Quotation.Create"));
            pages.CreateChildPermission(PermissionNames.Quotation_Update, L("Quotation.Update"));
            pages.CreateChildPermission(PermissionNames.Quotation_Delete, L("Quotation.Delete"));

            // PurchaseOrder
            pages.CreateChildPermission(PermissionNames.PurchaseOrder_Read, L("PurchaseOrder.Read"));
            pages.CreateChildPermission(PermissionNames.PurchaseOrder_Create, L("PurchaseOrder.Create"));
            pages.CreateChildPermission(PermissionNames.PurchaseOrder_Update, L("PurchaseOrder.Update"));
            pages.CreateChildPermission(PermissionNames.PurchaseOrder_Delete, L("PurchaseOrder.Delete"));
            pages.CreateChildPermission(PermissionNames.PurchaseOrder_ChangeStatus, L("PurchaseOrder.ChangeStatus"));

            // RBAC
            pages.CreateChildPermission(PermissionNames.AppUser_Read, L("AppUser.Read"));
            pages.CreateChildPermission(PermissionNames.AppRole_Read, L("AppRole.Read"));
            pages.CreateChildPermission(PermissionNames.AppUser_Create, L("AppUser.Create"));
            pages.CreateChildPermission(PermissionNames.AppRole_Create, L("AppRole.Create"));
            pages.CreateChildPermission(PermissionNames.AppUser_Update, L("AppUser.Update"));
            pages.CreateChildPermission(PermissionNames.AppRole_Update, L("AppRole.Update"));
            pages.CreateChildPermission(PermissionNames.AppUser_Delete, L("AppUser.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_Delete, L("AppRole.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_AssignPermissions, L("AppRole.AssignPermissions"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, directordemo2Consts.LocalizationSourceName);
        }
    }
}
