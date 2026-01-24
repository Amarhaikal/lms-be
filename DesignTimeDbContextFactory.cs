using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using QUANTM.Data;

namespace QUANTM
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Explicitly force Development environment behavior for design-time tools if not set
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            Console.WriteLine($"[DesignTimeFactory] Environment: {environment}");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

            var configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Mask password for safe logging
            var logSafeConnectionString = connectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                var parts = connectionString.Split(';');
                logSafeConnectionString = string.Join(";", parts.Select(p =>
                    p.Trim().StartsWith("Password", StringComparison.OrdinalIgnoreCase) ? "Password=*****" : p));
            }

            Console.WriteLine($"[DesignTimeFactory] Using Connection: {logSafeConnectionString}");


            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("5.7.0-mysql"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
