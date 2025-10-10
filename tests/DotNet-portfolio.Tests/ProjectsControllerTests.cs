using DotNet_portfolio.Controllers;
using DotNet_portfolio.Data;
using DotNet_portfolio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotNet_portfolio.Tests
{
    public class ProjectsControllerTests
    {
        private readonly DbContextOptions<PortfolioDbContext> _dbOptions;
        private readonly Mock<ILogger<ProjectsController>> _mockLogger;

        public ProjectsControllerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<PortfolioDbContext>()
                .UseInMemoryDatabase(databaseName: "PortfolioTestDb")
                .Options;

            _mockLogger = new Mock<ILogger<ProjectsController>>();
        }

        [Fact]
        public async Task GetProjects_ReturnsAllProjects()
        {
            using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Projects.AddRange(
                    new List<Project>
                    {
                        new Project
                        {
                            Id = 1,
                            Title = "Project 1",
                            Description = "Desc 1",
                            ProjectUrl = "http://example.com/1",
                            Tags = new List<string>(),
                        },
                        new Project
                        {
                            Id = 2,
                            Title = "Project 2",
                            Description = "Desc 2",
                            ProjectUrl = "http://example.com/2",
                            Tags = new List<string>(),
                        },
                    }
                );
                context.SaveChanges();
            }

            using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);

                var result = await controller.GetProjects();

                var actionResult = Assert.IsType<ActionResult<IEnumerable<Project>>>(result);
                var projects = Assert.IsAssignableFrom<IEnumerable<Project>>(actionResult.Value);
                Assert.Equal(2, projects.Count());
            }
        }

        [Fact]
        public async Task GetProjectById_ReturnsProject_WhenProjectExists()
        {
            using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Projects.Add(
                    new Project
                    {
                        Id = 1,
                        Title = "Project 1",
                        Description = "Desc 1",
                        ProjectUrl = "http://example.com/1",
                        Tags = new List<string>(),
                    }
                );
                context.SaveChanges();
            }

            using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);

                var result = await controller.GetProjectById(1);

                var actionResult = Assert.IsType<ActionResult<Project>>(result);
                var project = Assert.IsType<Project>(actionResult.Value);
                Assert.Equal(1, project.Id);
                Assert.Equal("Project 1", project.Title);
            }
        }

        [Fact]
        public async Task GetProjectById_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Database.EnsureDeleted();
            }

            using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);

                var result = await controller.GetProjectById(999);

                var actionResult = Assert.IsType<ActionResult<Project>>(result);
                Assert.IsType<NotFoundResult>(actionResult.Result);
            }
        }

        [Fact]
        public async Task CreateProject_ReturnsCreatedAtAction_WithNewProject()
        {
            using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Database.EnsureDeleted();
                var controller = new ProjectsController(context, _mockLogger.Object);
                var newProjectDto = new CreateProjectDto
                {
                    Title = "New Project",
                    Description = "New Description",
                    ProjectUrl = "http://example.com/new",
                };

                var result = await controller.CreateProject(newProjectDto);

                var actionResult = Assert.IsType<ActionResult<Project>>(result);
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(
                    actionResult.Result
                );
                var project = Assert.IsType<Project>(createdAtActionResult.Value);

                Assert.Equal("New Project", project.Title);
                Assert.Equal(1, context.Projects.Count());
            }
        }

        [Fact]
        public async Task UpdateProject_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Projects.Add(
                    new Project
                    {
                        Id = 1,
                        Title = "Old Title",
                        Description = "Old Desc",
                        ProjectUrl = "http://example.com/old",
                        Tags = new List<string>(),
                    }
                );
                context.SaveChanges();
            }

            using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);
                var updatedProjectDto = new UpdateProjectDto
                {
                    Title = "Updated Title",
                    Description = "Updated Description",
                    ProjectUrl = "http://example.com/updated",
                };

                var result = await controller.UpdateProject(1, updatedProjectDto);

                Assert.IsType<NoContentResult>(result);

                var updatedProject = await context.Projects.FindAsync(1);
                Assert.NotNull(updatedProject);
                Assert.Equal("Updated Title", updatedProject.Title);
            }
        }

        [Fact]
        public async Task DeleteProject_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Projects.Add(
                    new Project
                    {
                        Id = 1,
                        Title = "To Be Deleted",
                        Description = "Desc",
                        ProjectUrl = "http://example.com/delete",
                        Tags = new List<string>(),
                    }
                );
                context.SaveChanges();
            }

            using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);

                var result = await controller.DeleteProject(1);

                Assert.IsType<NoContentResult>(result);

                Assert.Equal(0, context.Projects.Count());
            }
        }
    }
}
