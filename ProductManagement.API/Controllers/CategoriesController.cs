using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.IService;
using ProductManagement.API.Model.ViewModels;
using ProductManagement.API.Model;
using ProductManagement.API.Service;

namespace ProductManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Categories")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>List of all categories</returns>
        /// <response code="200">Returns the list of categories</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get a specific category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        /// <response code="200">Returns the category</response>
        /// <response code="404">Category not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return category == null ? NotFound() : Ok(category);
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="category">Category to create</param>
        /// <returns>Created Category</returns>
        /// <response code="201">Category created successfully</response>
        /// <response code="400">Invalid input data</response>
        [HttpPost]
        [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> CreateCategory(CreateCategoryRequest category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newCategory = new Category
            {
                CategoryName = category.Name
            };

            var createdCategory = await _categoryService.CreateCategoryAsync(newCategory);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="category">Updated category data</param>
        /// <returns>Updated category</returns>
        /// <response code="200">Category updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Category not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> UpdateCategory(int id, UpdateCategoryRequest category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updateCategory = new Category
            {
                CategoryName = category.Name
            };

            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateCategory);
            return updatedCategory == null ? NotFound() : Ok(updatedCategory);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Category deleted successfully</response>
        /// <response code="404">Category not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
