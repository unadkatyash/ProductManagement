using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProductManagement.API.Data;
using ProductManagement.API.Hubs;
using ProductManagement.API.IService;
using ProductManagement.API.Model;

namespace ProductManagement.API.Service
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ProductHub> _hubContext;

        public ProductService(AppDbContext context, IHubContext<ProductHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(a => a.Category).OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Notify clients
            await _hubContext.Clients.Group("ProductUpdates")
                .SendAsync("ProductAdded", product);

            return product;
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return null;

            existingProduct.Name = product.Name;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Notify clients
            await _hubContext.Clients.Group("ProductUpdates")
                .SendAsync("ProductUpdated", existingProduct);

            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Notify clients
            await _hubContext.Clients.Group("ProductUpdates")
                .SendAsync("ProductDeleted", id);

            return true;
        }
    }
}
