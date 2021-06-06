using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PierogiesBot.Manager.Services
{
    public class DataInitializeHostedService : IHostedService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DataInitializeHostedService> _logger;

        public DataInitializeHostedService(ILogger<DataInitializeHostedService> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeData();
        }

        private async Task InitializeData()
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

            var migrations = pendingMigrations?.ToList() ?? new List<string>();
            _logger.LogInformation("Pending migrations [{0}]", string.Join(", ", migrations));

            if (pendingMigrations is not null && migrations.Any())
            {
                _logger.LogInformation("Migrating database");
                await _context.Database.MigrateAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}