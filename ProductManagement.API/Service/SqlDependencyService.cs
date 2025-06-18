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
        private SqlConnection _connection;
        private SqlCommand _command;

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
            try
            {
                // Dispose previous connection if exists
                _connection?.Dispose();
                _command?.Dispose();

                _connection = new SqlConnection(_connectionString);
                _connection.Open();

                // Use a more specific query that SqlDependency can handle
                _command = new SqlCommand(
                    "SELECT Id, Name, Category, Price, Stock, CreatedAt, UpdatedAt FROM dbo.Products",
                    _connection);

                _dependency = new SqlDependency(_command);
                _dependency.OnChange += OnDependencyChange;

                // Execute the command but don't dispose the connection
                using (var reader = _command.ExecuteReader())
                {
                    // Just execute to establish dependency
                }

                _logger.LogInformation("SqlDependency listening started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting SqlDependency");
            }
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
                if (_dependency != null)
                {
                    _dependency.OnChange -= OnDependencyChange;
                }

                _command?.Dispose();
                _connection?.Dispose();

                SqlDependency.Stop(_connectionString);
                _disposed = true;
            }
        }
    }
}