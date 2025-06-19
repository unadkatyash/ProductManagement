using System.ComponentModel.DataAnnotations;

namespace ProductManagement.API.Model.ViewModels
{
    public class CreateProductRequest
    {
        /// <summary>
        /// Product name
        /// </summary>
        /// <example>Laptop</example>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product category
        /// </summary>
        /// <example>Electronics</example>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        /// <example>999.99</example>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        /// <summary>
        /// Stock quantity
        /// </summary>
        /// <example>50</example>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }
    }
}
