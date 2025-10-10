using DotNet_portfolio.Controllers;
using DotNet_portfolio.Data;
using DotNet_portfolio.Models;
using DotNet_portfolio.Services;
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
        private readonly Mock<IDbErrorService> _mockDbErrorService;

        public ProjectsControllerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<PortfolioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockLogger = new Mock<ILogger<ProjectsController>>();
            _mockDbErrorService = new Mock<IDbErrorService>();
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
                var controller = new ProjectsController(
                    context,
                    _mockLogger.Object,
                    _mockDbErrorService.Object
                );
                var result = await controller.GetProjects();

                var actionResult = Assert.IsType<ActionResult<IEnumerable<Project>>>(result);
                var projects = Assert.IsAssignableFrom<IEnumerable<Project>>(actionResult.Value);
                Assert.Equal(2, projects.Count());
            }
        }

        [Fact]
        public void GetPortfolioMessage_ReturnsOkResultWithMessage()
        {
            using var context = new PortfolioDbContext(_dbOptions);
            var controller = new ProjectsController(
                context,
                _mockLogger.Object,
                _mockDbErrorService.Object
            );

            var result = controller.GetPortfolioMessage();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            Assert.NotNull(value);
            var messageProperty = value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            var message = messageProperty.GetValue(value, null) as string;
            Assert.Equal("Portfolio API is running successfully.", message);
        }

        [Fact]
        public async Task CreateProject_ReturnsConflict_WhenTitleAlreadyExists()
        {
            var mockContext = new Mock<PortfolioDbContext>(_dbOptions);
            var mockSet = new Mock<DbSet<Project>>();
            var dbUpdateException = new DbUpdateException("Simulated exception", new Exception());

            mockContext.Setup(m => m.Projects).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(default)).ThrowsAsync(dbUpdateException);

            _mockDbErrorService
                .Setup(s => s.IsUniqueConstraintViolation(dbUpdateException))
                .Returns(true);

            var controller = new ProjectsController(
                mockContext.Object,
                _mockLogger.Object,
                _mockDbErrorService.Object
            );
            var projectDto = new CreateProjectDto
            {
                Title = "Existing Title",
                Description = "Some description",
                ProjectUrl = "http://example.com",
            };

            var result = await controller.CreateProject(projectDto);

            var actionResult = Assert.IsType<ActionResult<Project>>(result);
            Assert.IsType<ConflictObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task UpdateProject_ReturnsConflict_WhenNewTitleAlreadyExists()
        {
            var projectToUpdate = new Project
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                ProjectUrl = "http://example.com/old",
            };
            var mockSet = new Mock<DbSet<Project>>();
            var dbUpdateException = new DbUpdateException("Simulated exception", new Exception());

            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(projectToUpdate);

            var mockContext = new Mock<PortfolioDbContext>(_dbOptions);
            mockContext.Setup(m => m.Projects).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(default)).ThrowsAsync(dbUpdateException);

            _mockDbErrorService
                .Setup(s => s.IsUniqueConstraintViolation(dbUpdateException))
                .Returns(true);

            var controller = new ProjectsController(
                mockContext.Object,
                _mockLogger.Object,
                _mockDbErrorService.Object
            );
            var projectDto = new UpdateProjectDto
            {
                Title = "Existing Title",
                Description = "New",
                ProjectUrl = "http://a.com",
            };

            var result = await controller.UpdateProject(1, projectDto);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task CreateProjects_ReturnsConflict_WhenAnyTitleAlreadyExists()
        {
            var mockContext = new Mock<PortfolioDbContext>(_dbOptions);
            var mockSet = new Mock<DbSet<Project>>();
            var dbUpdateException = new DbUpdateException("Simulated exception", new Exception());

            mockContext.Setup(m => m.Projects).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(default)).ThrowsAsync(dbUpdateException);

            _mockDbErrorService
                .Setup(s => s.IsUniqueConstraintViolation(dbUpdateException))
                .Returns(true);

            var controller = new ProjectsController(
                mockContext.Object,
                _mockLogger.Object,
                _mockDbErrorService.Object
            );
            var projectsDto = new List<CreateProjectDto>
            {
                new()
                {
                    Title = "New Project",
                    Description = "Desc",
                    ProjectUrl = "http://a.com",
                },
            };

            var result = await controller.CreateProjects(projectsDto);

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Project>>>(result);
            Assert.IsType<ConflictObjectResult>(actionResult.Result);
        }
    }
}
