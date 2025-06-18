using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System;
using ProductManagement.API.Data;
using ProductManagement.API.IService;
using ProductManagement.API.Service;
using ProductManagement.API.Model;
using ProductManagement.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Product Management API",
        Version = "v1",
        Description = "A product management API for managing products with SignalR integration",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        }
    });

    // Add SignalR Hub documentation
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.DocInclusionPredicate((name, api) => true);
});

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Product Management API Documentation";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
    });
}

app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRouting();
app.MapControllers();
app.MapHub<ProductHub>("/productHub");

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product { Name = "Laptop", Category = "Electronics", Price = 999.99m, Stock = 50 },
            new Product { Name = "Mouse", Category = "Electronics", Price = 25.99m, Stock = 100 },
            new Product { Name = "Keyboard", Category = "Electronics", Price = 75.50m, Stock = 75 },
            new Product { Name = "Monitor", Category = "Electronics", Price = 299.99m, Stock = 30 },
            new Product { Name = "Headphones", Category = "Audio", Price = 199.99m, Stock = 60 }
        );
        context.SaveChanges();
    }
}

app.Run();