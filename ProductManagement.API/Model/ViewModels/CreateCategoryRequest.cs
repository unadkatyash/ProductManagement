using System.ComponentModel.DataAnnotations;

namespace ProductManagement.API.Model.ViewModels
{
    public class CreateCategoryRequest
    {
        /// <summary>
        /// Category name
        /// </summary>
        /// <example>Audio</example>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;
    }
}
