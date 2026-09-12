using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using directordemo2.EntityFrameworkCore;
using directordemo2.EntityFrameworkCore.Seed;
using directordemo2.Entities;

namespace directordemo2.Web.Host
{
    /// <summary>
    /// Background service that runs migration + seed once at startup
    /// without blocking the HTTP pipeline.
    /// </summary>
    public class MigrationHostedService : IHostedService
    {
        private readonly IConfiguration _config;

        public MigrationHostedService(IConfiguration config)
        {
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var connStr = _config.GetConnectionString("Default") ?? "";
            if (string.IsNullOrEmpty(connStr)) return;

            // Run in background to avoid blocking host startup (prevents EF tooling timeout)
            _ = Task.Run(async () =>
            {
                // Small delay to ensure host is fully started before DB operations
                await Task.Delay(1000, cancellationToken);
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseNpgsql(connStr);

                    using (var db = new directordemo2DbContext(optionsBuilder.Options))
                    {
                        // EnsureCreated bos veritabaninda en guncel semayi kurar ve true doner.
                        // Dolu veritabaninda hicbir sey yapmaz — o durumda bekleyen migration'lar
                        // SchemaMigrations.Apply icinde sirayla calistirilir.
                        var freshlyCreated = db.Database.EnsureCreated();
                        Console.WriteLine("[Migration] Migration tanimi yok — sema EnsureCreated ile yonetiliyor.");
                        Console.WriteLine("[Migration] Database is up to date.");

                        // Seed sample data — wrapped in its own try so a failure here doesn't block RBAC seed below.
                        try
                        {
                    if (!db.Departments.Any())
                    {
                        db.Departments.AddRange(
                    new Department { Id = 1, Code = "ABC-001", Name = "Alice Johnson", AnnualBudget = 99.99m, IsActive = true },
                    new Department { Id = 2, Code = "XYZ-002", Name = "Bob Smith", AnnualBudget = 149.50m, IsActive = false }
                        );
                    }
                    if (!db.Employees.Any())
                    {
                        db.Employees.AddRange(
                    new Employee { Id = 3, RegistrationNumber = "ABC-001", FullName = "Alice Johnson", Email = "alice@example.com", Title = "Introduction to Physics", IsActive = true, DepartmentId = 1 },
                    new Employee { Id = 4, RegistrationNumber = "XYZ-002", FullName = "Bob Smith", Email = "bob@example.com", Title = "Advanced Mathematics", IsActive = false, DepartmentId = 2 }
                        );
                    }
                    if (!db.Suppliers.Any())
                    {
                        db.Suppliers.AddRange(
                    new Supplier { Id = 5, Code = "ABC-001", Name = "Alice Johnson", TaxNumber = "ABC-001", ContactPerson = "Sample Item 1", Email = "alice@example.com", IsActive = true },
                    new Supplier { Id = 6, Code = "XYZ-002", Name = "Bob Smith", TaxNumber = "XYZ-002", ContactPerson = "Sample Item 2", Email = "bob@example.com", IsActive = false }
                        );
                    }
                    if (!db.ExpenseCategories.Any())
                    {
                        db.ExpenseCategories.AddRange(
                    new ExpenseCategory { Id = 7, Code = "ABC-001", Name = "Alice Johnson", IsActive = true },
                    new ExpenseCategory { Id = 8, Code = "XYZ-002", Name = "Bob Smith", IsActive = false }
                        );
                    }
                    if (!db.PurchaseRequests.Any())
                    {
                        db.PurchaseRequests.AddRange(
                    new PurchaseRequest { Id = 9, RequestNumber = "ABC-001", Justification = "Sample Item 1", TotalAmount = 99.99m, NeededDate = new DateTime(2024, 3, 15), ApprovedDate = new DateTime(2024, 3, 15), RejectionReason = "Sample Item 1", Status = (PurchaseRequestStatus)0, EmployeeId = 3, DepartmentId = 1, ExpenseCategoryId = 7 },
                    new PurchaseRequest { Id = 10, RequestNumber = "XYZ-002", Justification = "Sample Item 2", TotalAmount = 149.50m, NeededDate = new DateTime(2024, 6, 20), ApprovedDate = new DateTime(2024, 6, 20), RejectionReason = "Sample Item 2", Status = (PurchaseRequestStatus)1, EmployeeId = 4, DepartmentId = 2, ExpenseCategoryId = 8 }
                        );
                    }
                    if (!db.PurchaseRequestItems.Any())
                    {
                        db.PurchaseRequestItems.AddRange(
                    new PurchaseRequestItem { Id = 11, ProductName = "Alice Johnson", Quantity = 99.99m, UnitPrice = 99.99m, LineTotal = 99.99m, PurchaseRequestId = 9 },
                    new PurchaseRequestItem { Id = 12, ProductName = "Bob Smith", Quantity = 149.50m, UnitPrice = 149.50m, LineTotal = 149.50m, PurchaseRequestId = 10 }
                        );
                    }
                    if (!db.Quotations.Any())
                    {
                        db.Quotations.AddRange(
                    new Quotation { Id = 13, QuotationDate = new DateTime(2024, 3, 15), ValidUntil = new DateTime(2024, 3, 15), Amount = 99.99m, IsSelected = true, PurchaseRequestId = 9, SupplierId = 5 },
                    new Quotation { Id = 14, QuotationDate = new DateTime(2024, 6, 20), ValidUntil = new DateTime(2024, 6, 20), Amount = 149.50m, IsSelected = false, PurchaseRequestId = 10, SupplierId = 6 }
                        );
                    }
                    if (!db.PurchaseOrders.Any())
                    {
                        db.PurchaseOrders.AddRange(
                    new PurchaseOrder { Id = 15, OrderNumber = "ABC-001", OrderDate = new DateTime(2024, 3, 15), ExpectedDeliveryDate = new DateTime(2024, 3, 15), DeliveryDate = new DateTime(2024, 3, 15), OrderAmount = 99.99m, Notes = "Lorem ipsum dolor sit amet", Status = (PurchaseOrderStatus)0, PurchaseRequestId = 9, SupplierId = 5 },
                    new PurchaseOrder { Id = 16, OrderNumber = "XYZ-002", OrderDate = new DateTime(2024, 6, 20), ExpectedDeliveryDate = new DateTime(2024, 6, 20), DeliveryDate = new DateTime(2024, 6, 20), OrderAmount = 149.50m, Notes = "Consectetur adipiscing elit", Status = (PurchaseOrderStatus)1, PurchaseRequestId = 10, SupplierId = 6 }
                        );
                    }
                            db.SaveChanges();
                            Console.WriteLine("[Seed] Sample data created.");
                        }
                        catch (Exception sampleEx)
                        {
                            Console.WriteLine($"[Seed] Sample data skipped: {sampleEx.GetType().Name}: {sampleEx.Message}");
                            // Carry on — RBAC seed must still run so admin/123qwe is usable.
                        }
                        // Sync identity sequences to MAX(Id). Seeded rows carry explicit Ids which do NOT
                        // advance Postgres identity sequences → nextval collides with a seed row and the
                        // first few inserts fail with a duplicate-key 500. Runs every startup; idempotent.
                        try
                        {
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Departments\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Departments\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Employees\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Employees\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Suppliers\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Suppliers\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"ExpenseCategories\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"ExpenseCategories\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"PurchaseRequests\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"PurchaseRequests\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"PurchaseRequestItems\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"PurchaseRequestItems\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Quotations\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Quotations\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"PurchaseOrders\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"PurchaseOrders\") + 1, false);");
                            Console.WriteLine("[Seed] Identity sequences synced.");
                        }
                        catch (Exception seqEx)
                        {
                            Console.WriteLine($"[Seed] Sequence sync skipped: {seqEx.GetType().Name}: {seqEx.Message}");
                        }
                    }
                    // RBAC seed (Admin/User roles + permissions + admin user) runs through ABP DI
                    // so PermissionRegistry can be injected. SeedHelper is idempotent.
                    SeedHelper.SeedHostDb(Abp.Dependency.IocManager.Instance);
                    Console.WriteLine("[Seed] RBAC seed complete (Admin role + admin user).");
                }
                catch (Exception ex)
                {
                    // Full diagnostic — surface the real cause so silent seed failures are debuggable.
                    Console.WriteLine($"[Migration] FAILED: {ex.GetType().Name}: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Migration] InnerException: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                    Console.WriteLine("[Migration] StackTrace:");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("[Migration] App continues without migration — admin user will not exist.");
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class Program
    {
        // Runtime entry: WebHost is required because ABP Startup returns IServiceProvider.
        public static void Main(string[] args)
        {
            // Npgsql 7+ requires UTC DateTimes — enable legacy behavior for ABP compatibility
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        // Design-time entry for EF Core tools (dotnet ef migrations).
        // Without this, EF tools wait 5 minutes for IHost build (resolver default timeout)
        // and then SIGTERM any running dotnet process — killing live dev servers.
        // We expose a minimal IHost that EF tools resolve in milliseconds; the actual
        // DbContext is built by IDesignTimeDbContextFactory in the EntityFrameworkCore project.
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
    }
}
