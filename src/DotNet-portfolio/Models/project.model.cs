using System.ComponentModel.DataAnnotations;

namespace DotNet_portfolio.Models
{
    /// <summary>
    /// Represents a project in the portfolio.
    /// </summary>
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A title is required.")]
        [StringLength(100, ErrorMessage = "The title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "A description is required.")]
        public required string Description { get; set; }

        [Url(ErrorMessage = "The image URL is not a valid URL.")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "A project URL is required.")]
        [Url(ErrorMessage = "The project URL is not a valid URL.")]
        public required string ProjectUrl { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}
