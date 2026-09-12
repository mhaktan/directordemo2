using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace directordemo2.EntityFrameworkCore
{
    public class directordemo2DbContextFactory : IDesignTimeDbContextFactory<directordemo2DbContext>
    {
        public directordemo2DbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../directordemo2.Web.Host"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connStr = configuration.GetConnectionString("Default");
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(connStr);
            return new directordemo2DbContext(builder.Options);
        }
    }
}
