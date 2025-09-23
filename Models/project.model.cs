namespace DotNet_portfolio.Models
{
    /// <summary>
    /// Represents a project in the portfolio.
    /// </summary>
    public class Project
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? ImageUrl { get; set; }
        public required string ProjectUrl { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
