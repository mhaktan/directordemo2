using System.Collections.Generic;
using Abp.Dependency;

namespace directordemo2.Authorization
{
    /// <summary>Single permission descriptor — name, group (entity), description.</summary>
    public class PermissionInfo
    {
        public string Name { get; }
        public string Group { get; }
        public string Description { get; }
        public bool IsRbac { get; }

        public PermissionInfo(string name, string group, string description, bool isRbac)
        {
            Name = name; Group = group; Description = description; IsRbac = isRbac;
        }
    }

    public interface IPermissionRegistry
    {
        IReadOnlyList<PermissionInfo> All { get; }
    }

    public class PermissionRegistry : IPermissionRegistry, ISingletonDependency
    {
        public IReadOnlyList<PermissionInfo> All { get; } = new List<PermissionInfo>
        {
            new PermissionInfo("Department.Read", "Department", "Read Department", false),
            new PermissionInfo("Department.Create", "Department", "Create Department", false),
            new PermissionInfo("Department.Update", "Department", "Update Department", false),
            new PermissionInfo("Department.Delete", "Department", "Delete Department", false),
            new PermissionInfo("Employee.Read", "Employee", "Read Employee", false),
            new PermissionInfo("Employee.Create", "Employee", "Create Employee", false),
            new PermissionInfo("Employee.Update", "Employee", "Update Employee", false),
            new PermissionInfo("Employee.Delete", "Employee", "Delete Employee", false),
            new PermissionInfo("Supplier.Read", "Supplier", "Read Supplier", false),
            new PermissionInfo("Supplier.Create", "Supplier", "Create Supplier", false),
            new PermissionInfo("Supplier.Update", "Supplier", "Update Supplier", false),
            new PermissionInfo("Supplier.Delete", "Supplier", "Delete Supplier", false),
            new PermissionInfo("ExpenseCategory.Read", "ExpenseCategory", "Read ExpenseCategory", false),
            new PermissionInfo("ExpenseCategory.Create", "ExpenseCategory", "Create ExpenseCategory", false),
            new PermissionInfo("ExpenseCategory.Update", "ExpenseCategory", "Update ExpenseCategory", false),
            new PermissionInfo("ExpenseCategory.Delete", "ExpenseCategory", "Delete ExpenseCategory", false),
            new PermissionInfo("PurchaseRequest.Read", "PurchaseRequest", "Read PurchaseRequest", false),
            new PermissionInfo("PurchaseRequest.Create", "PurchaseRequest", "Create PurchaseRequest", false),
            new PermissionInfo("PurchaseRequest.Update", "PurchaseRequest", "Update PurchaseRequest", false),
            new PermissionInfo("PurchaseRequest.Delete", "PurchaseRequest", "Delete PurchaseRequest", false),
            new PermissionInfo("PurchaseRequest.ChangeStatus", "PurchaseRequest", "Change PurchaseRequest status", false),
            new PermissionInfo("PurchaseRequestItem.Read", "PurchaseRequestItem", "Read PurchaseRequestItem", false),
            new PermissionInfo("PurchaseRequestItem.Create", "PurchaseRequestItem", "Create PurchaseRequestItem", false),
            new PermissionInfo("PurchaseRequestItem.Update", "PurchaseRequestItem", "Update PurchaseRequestItem", false),
            new PermissionInfo("PurchaseRequestItem.Delete", "PurchaseRequestItem", "Delete PurchaseRequestItem", false),
            new PermissionInfo("Quotation.Read", "Quotation", "Read Quotation", false),
            new PermissionInfo("Quotation.Create", "Quotation", "Create Quotation", false),
            new PermissionInfo("Quotation.Update", "Quotation", "Update Quotation", false),
            new PermissionInfo("Quotation.Delete", "Quotation", "Delete Quotation", false),
            new PermissionInfo("PurchaseOrder.Read", "PurchaseOrder", "Read PurchaseOrder", false),
            new PermissionInfo("PurchaseOrder.Create", "PurchaseOrder", "Create PurchaseOrder", false),
            new PermissionInfo("PurchaseOrder.Update", "PurchaseOrder", "Update PurchaseOrder", false),
            new PermissionInfo("PurchaseOrder.Delete", "PurchaseOrder", "Delete PurchaseOrder", false),
            new PermissionInfo("PurchaseOrder.ChangeStatus", "PurchaseOrder", "Change PurchaseOrder status", false),
            new PermissionInfo("AppUser.Read", "AppUser", "Read users", true),
            new PermissionInfo("AppRole.Read", "AppRole", "Read roles", true),
            new PermissionInfo("AppUser.Create", "AppUser", "Create users", true),
            new PermissionInfo("AppRole.Create", "AppRole", "Create roles", true),
            new PermissionInfo("AppUser.Update", "AppUser", "Update users", true),
            new PermissionInfo("AppRole.Update", "AppRole", "Update roles", true),
            new PermissionInfo("AppUser.Delete", "AppUser", "Delete users", true),
            new PermissionInfo("AppRole.Delete", "AppRole", "Delete roles", true),
            new PermissionInfo("AppRole.AssignPermissions", "AppRole", "Assign permissions to roles", true),
        };
    }
}
