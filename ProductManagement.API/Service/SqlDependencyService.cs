using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductManagement.API.Data;
using ProductManagement.API.Hubs;
using ProductManagement.API.Model;
using System.Data;

namespace ProductManagement.API.Service
{
    public class SqlDependencyService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SqlDependencyService> _logger;
        private readonly string _connectionString;
        private SqlDependency _dependency;
        private bool _disposed = false;

        public SqlDependencyService(IServiceProvider serviceProvider,
            ILogger<SqlDependencyService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            SqlDependency.Start(_connectionString);
            StartListening();
            _logger.LogInformation("SqlDependency service started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            SqlDependency.Stop(_connectionString);
            _logger.LogInformation("SqlDependency service stopped");
            return Task.CompletedTask;
        }

        private void StartListening()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand(
                "SELECT Id, Name, Category, Price, Stock, CreatedAt, UpdatedAt FROM Products",
                connection);

            _dependency = new SqlDependency(command);
            _dependency.OnChange += OnDependencyChange;

            // Execute the command to establish the dependency
            command.ExecuteReader();
        }

        private async void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                _logger.LogInformation("Database change detected, notifying clients...");

                using var scope = _serviceProvider.CreateScope();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ProductHub>>();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    // Get updated products and notify clients
                    var products = await context.Products.OrderBy(p => p.Name).ToListAsync();
                    await hubContext.Clients.Group("ProductUpdates")
                        .SendAsync("ProductsRefreshed", products);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling database change notification");
                }

                // Restart listening for next change
                StartListening();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _dependency.OnChange -= OnDependencyChange;
                SqlDependency.Stop(_connectionString);
                _disposed = true;
            }
        }
    }
}