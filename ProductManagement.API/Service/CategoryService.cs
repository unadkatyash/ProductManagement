using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProductManagement.API.Data;
using ProductManagement.API.Hubs;
using ProductManagement.API.IService;
using ProductManagement.API.Model;

namespace ProductManagement.API.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ProductHub> _hubContext;

        public CategoryService(AppDbContext context, IHubContext<ProductHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(p => p.Id).ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Notify clients
            await _hubContext.Clients.Group("ProductUpdates")
                .SendAsync("CategoryAdded", category);

            return category;
        }

        public async Task<Category?> UpdateCategoryAsync(int id, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null) return null;

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Notify clients
            await _hubContext.Clients.Group("ProductUpdates")
                .SendAsync("CategoryUpdated", existingCategory);

            return existingCategory;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            // Notify clients
            await _hubContext.Clients.Group("ProductUpdates")
                .SendAsync("CategoryDeleted", id);

            return true;
        }
    }
}
