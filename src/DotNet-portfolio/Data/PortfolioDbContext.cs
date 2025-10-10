using DotNet_portfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet_portfolio.Data
{
    /// <summary>
    /// The database context for the portfolio application.
    /// </summary>
    public class PortfolioDbContext : DbContext
    {
        public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }
    }
}
