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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockLogger = new Mock<ILogger<ProjectsController>>();
        }

        [Fact]
        public async Task GetProjects_ReturnsAllProjects()
        {
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Projects.AddRange(
                    new Project
                    {
                        Id = 1,
                        Title = "Project 1",
                        Description = "Desc 1",
                        ProjectUrl = "http://example.com/1",
                    },
                    new Project
                    {
                        Id = 2,
                        Title = "Project 2",
                        Description = "Desc 2",
                        ProjectUrl = "http://example.com/2",
                    }
                );
                await context.SaveChangesAsync();
            }

            await using (var context = new PortfolioDbContext(_dbOptions))
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
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Projects.Add(
                    new Project
                    {
                        Id = 1,
                        Title = "Project 1",
                        Description = "Desc 1",
                        ProjectUrl = "http://example.com/1",
                    }
                );
                await context.SaveChangesAsync();
            }

            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);
                var result = await controller.GetProjectById(1);

                var actionResult = Assert.IsType<ActionResult<Project>>(result);
                var project = Assert.IsType<Project>(actionResult.Value);
                Assert.Equal(1, project.Id);
            }
        }

        [Fact]
        public async Task GetProjectById_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);
                var result = await controller.GetProjectById(999);

                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Fact]
        public async Task CreateProject_ReturnsCreatedAtAction_WithNewProject()
        {
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
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
                Assert.Equal(1, await context.Projects.CountAsync());
            }
        }

        [Fact]
        public async Task UpdateProject_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Projects.Add(
                    new Project
                    {
                        Id = 1,
                        Title = "Old Title",
                        Description = "Old Desc",
                        ProjectUrl = "http://example.com/old",
                    }
                );
                await context.SaveChangesAsync();
            }

            await using (var context = new PortfolioDbContext(_dbOptions))
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
                Assert.Equal("Updated Title", updatedProject.Title);
            }
        }

        [Fact]
        public async Task DeleteProject_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                context.Projects.Add(
                    new Project
                    {
                        Id = 1,
                        Title = "To Be Deleted",
                        Description = "Desc",
                        ProjectUrl = "http://example.com/delete",
                    }
                );
                await context.SaveChangesAsync();
            }

            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);
                var result = await controller.DeleteProject(1);

                Assert.IsType<NoContentResult>(result);
                Assert.Equal(0, await context.Projects.CountAsync());
            }
        }

        [Fact]
        public async Task UpdateProject_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);
                var updatedDto = new UpdateProjectDto
                {
                    Title = "Title",
                    Description = "Desc",
                    ProjectUrl = "http://a.com",
                };

                var result = await controller.UpdateProject(999, updatedDto);

                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeleteProject_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);

                var result = await controller.DeleteProject(999);

                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task CreateProjects_ReturnsBadRequest_WhenInputIsNull()
        {
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);

                var result = await controller.CreateProjects(null);

                var actionResult = Assert.IsType<ActionResult<IEnumerable<Project>>>(result);
                Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            }
        }

        [Fact]
        public async Task CreateProjects_SuccessfullyCreatesMultipleProjects()
        {
            await using (var context = new PortfolioDbContext(_dbOptions))
            {
                var controller = new ProjectsController(context, _mockLogger.Object);
                var projectDtos = new List<CreateProjectDto>
                {
                    new CreateProjectDto
                    {
                        Title = "Bulk Project 1",
                        Description = "Desc 1",
                        ProjectUrl = "http://b.com/1",
                    },
                    new CreateProjectDto
                    {
                        Title = "Bulk Project 2",
                        Description = "Desc 2",
                        ProjectUrl = "http://b.com/2",
                    },
                };

                var result = await controller.CreateProjects(projectDtos);

                var actionResult = Assert.IsType<ActionResult<IEnumerable<Project>>>(result);
                Assert.IsType<OkObjectResult>(actionResult.Result);
                Assert.Equal(2, await context.Projects.CountAsync());
            }
        }
    }
}
