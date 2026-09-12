using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore;
using directordemo2.Entities;

namespace directordemo2.EntityFrameworkCore
{
    public class directordemo2DbContext : AbpDbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ApprovalRecord> ApprovalRecords { get; set; }
        public DbSet<StatusChangeLog> StatusChangeLogs { get; set; }


        public directordemo2DbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Department 1:N Employee
            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Department)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee 1:N PurchaseRequest
            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.PurchaseRequests)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department 1:N PurchaseRequest
            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(x => x.Department)
                .WithMany(x => x.PurchaseRequests)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExpenseCategory 1:N PurchaseRequest
            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(x => x.ExpenseCategory)
                .WithMany(x => x.PurchaseRequests)
                .HasForeignKey(x => x.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // PurchaseRequest 1:N PurchaseRequestItem
            modelBuilder.Entity<PurchaseRequestItem>()
                .HasOne(x => x.PurchaseRequest)
                .WithMany(x => x.PurchaseRequestItems)
                .HasForeignKey(x => x.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // PurchaseRequest 1:N Quotation
            modelBuilder.Entity<Quotation>()
                .HasOne(x => x.PurchaseRequest)
                .WithMany(x => x.Quotations)
                .HasForeignKey(x => x.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Supplier 1:N Quotation
            modelBuilder.Entity<Quotation>()
                .HasOne(x => x.Supplier)
                .WithMany(x => x.Quotations)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // PurchaseRequest 1:N PurchaseOrder
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(x => x.PurchaseRequest)
                .WithMany(x => x.PurchaseOrders)
                .HasForeignKey(x => x.PurchaseRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Supplier 1:N PurchaseOrder
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(x => x.Supplier)
                .WithMany(x => x.PurchaseOrders)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);


            // RBAC: AppUser N:N AppRole via UserRole junction
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // RolePermission: AppRole 1:N RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionName })
                .IsUnique();

            // AppRole.Name unique
            modelBuilder.Entity<AppRole>()
                .HasIndex(r => r.Name)
                .IsUnique();

        }
    }
}
