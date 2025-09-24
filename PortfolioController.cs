using DotNet_portfolio.Data;
using DotNet_portfolio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DotNet_portfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly PortfolioDbContext _context;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(PortfolioDbContext context, ILogger<ProjectsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProjectById(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(CreateProjectDto projectDto)
        {
            var project = new Project
            {
                Title = projectDto.Title,
                Description = projectDto.Description,
                ImageUrl = projectDto.ImageUrl,
                ProjectUrl = projectDto.ProjectUrl,
                Tags = projectDto.Tags
            };
            _logger.LogInformation("Создан новый объект Project в памяти. Его ID: {ProjectId}", project.Id);
            _context.Projects.Add(project);
            var entry = _context.Entry(project);
            _logger.LogInformation(
                "EF Change Tracker видит проект '{ProjectTitle}' с ID {ProjectId} в состоянии '{State}'",
                entry.Entity.Title,
                entry.Entity.Id,
                entry.State
            );

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException postgresEx && postgresEx.SqlState == "23505")
            {
                _logger.LogError(ex, "Поймана ошибка уникальности при сохранении.");
                return Conflict(new { message = $"Проект с названием '{project.Title}' уже существует." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла необработанная ошибка при сохранении проекта.");
                throw;
            }

            _logger.LogInformation("Проект успешно сохранен. Новый ID от базы данных: {ProjectId}", project.Id);
            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<Project>>> CreateProjects(IEnumerable<CreateProjectDto> projectDtos)
        {
            if (projectDtos == null || !projectDtos.Any())
            {
                return BadRequest("Список проектов не может быть пустым.");
            }

            var projects = projectDtos.Select(dto => new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                ProjectUrl = dto.ProjectUrl,
                Tags = dto.Tags
            }).ToList();

            _context.Projects.AddRange(projects);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException postgresEx && postgresEx.SqlState == "23505")
            {
                return Conflict(new { message = "Один или несколько проектов имеют названия, которые уже существуют." });
            }

            return Ok(projects);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, UpdateProjectDto projectDto)
        {
            var projectToUpdate = await _context.Projects.FindAsync(id);

            if (projectToUpdate == null)
            {
                return NotFound();
            }

            projectToUpdate.Title = projectDto.Title;
            projectToUpdate.Description = projectDto.Description;
            projectToUpdate.ImageUrl = projectDto.ImageUrl;
            projectToUpdate.ProjectUrl = projectDto.ProjectUrl;
            projectToUpdate.Tags = projectDto.Tags;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException postgresEx && postgresEx.SqlState == "23505")
            {
                return Conflict(new { message = $"Проект с названием '{projectDto.Title}' уже существует." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}