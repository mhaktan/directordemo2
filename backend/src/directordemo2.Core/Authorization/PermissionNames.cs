namespace directordemo2.Authorization
{
    public static class PermissionNames
    {
        public const string Pages = "Pages";

        // Department
        public const string Department_Read = "Department.Read";
        public const string Department_Create = "Department.Create";
        public const string Department_Update = "Department.Update";
        public const string Department_Delete = "Department.Delete";

        // Employee
        public const string Employee_Read = "Employee.Read";
        public const string Employee_Create = "Employee.Create";
        public const string Employee_Update = "Employee.Update";
        public const string Employee_Delete = "Employee.Delete";

        // Supplier
        public const string Supplier_Read = "Supplier.Read";
        public const string Supplier_Create = "Supplier.Create";
        public const string Supplier_Update = "Supplier.Update";
        public const string Supplier_Delete = "Supplier.Delete";

        // ExpenseCategory
        public const string ExpenseCategory_Read = "ExpenseCategory.Read";
        public const string ExpenseCategory_Create = "ExpenseCategory.Create";
        public const string ExpenseCategory_Update = "ExpenseCategory.Update";
        public const string ExpenseCategory_Delete = "ExpenseCategory.Delete";

        // PurchaseRequest
        public const string PurchaseRequest_Read = "PurchaseRequest.Read";
        public const string PurchaseRequest_Create = "PurchaseRequest.Create";
        public const string PurchaseRequest_Update = "PurchaseRequest.Update";
        public const string PurchaseRequest_Delete = "PurchaseRequest.Delete";
        public const string PurchaseRequest_ChangeStatus = "PurchaseRequest.ChangeStatus";

        // PurchaseRequestItem
        public const string PurchaseRequestItem_Read = "PurchaseRequestItem.Read";
        public const string PurchaseRequestItem_Create = "PurchaseRequestItem.Create";
        public const string PurchaseRequestItem_Update = "PurchaseRequestItem.Update";
        public const string PurchaseRequestItem_Delete = "PurchaseRequestItem.Delete";

        // Quotation
        public const string Quotation_Read = "Quotation.Read";
        public const string Quotation_Create = "Quotation.Create";
        public const string Quotation_Update = "Quotation.Update";
        public const string Quotation_Delete = "Quotation.Delete";

        // PurchaseOrder
        public const string PurchaseOrder_Read = "PurchaseOrder.Read";
        public const string PurchaseOrder_Create = "PurchaseOrder.Create";
        public const string PurchaseOrder_Update = "PurchaseOrder.Update";
        public const string PurchaseOrder_Delete = "PurchaseOrder.Delete";
        public const string PurchaseOrder_ChangeStatus = "PurchaseOrder.ChangeStatus";

        // RBAC management
        public const string AppUser_Read = "AppUser.Read";
        public const string AppRole_Read = "AppRole.Read";
        public const string AppUser_Create = "AppUser.Create";
        public const string AppRole_Create = "AppRole.Create";
        public const string AppUser_Update = "AppUser.Update";
        public const string AppRole_Update = "AppRole.Update";
        public const string AppUser_Delete = "AppUser.Delete";
        public const string AppRole_Delete = "AppRole.Delete";
        public const string AppRole_AssignPermissions = "AppRole.AssignPermissions";

    }
}
