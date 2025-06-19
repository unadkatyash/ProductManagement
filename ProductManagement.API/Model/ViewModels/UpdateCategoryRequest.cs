using System.ComponentModel.DataAnnotations;

namespace ProductManagement.API.Model.ViewModels
{
    public class UpdateCategoryRequest
    {
        /// <summary>
        /// Category name
        /// </summary>
        /// <example>Updated Laptop</example>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;
    }
}
