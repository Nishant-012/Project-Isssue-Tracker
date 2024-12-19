using IssueTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Domain;
using Models.DTO;

namespace IssueTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProjectController> _logger;
        public ProjectController(ApplicationDbContext context, ILogger<ProjectController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/Project
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Project>> CreateProject([FromBody] ProjectDTO projectDto)
        {
            if (projectDto == null || string.IsNullOrEmpty(projectDto.Name))
            {
                _logger.LogWarning("Project creation failed: Project name is required.");
                return BadRequest("Project name is required.");
            }

            // Create a new Project entity from the DTO
            var project = new Project
            {
                Name = projectDto.Name
            };

            // Add the project to the database
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Project created successfully with ID {ProjectId}", project.Id);

            // Return the created project
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        // GET: api/Project/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            // Find the project by ID, including related issues if needed
            _logger.LogInformation("Fetching project with ID {ProjectId}", id);
            var project = await _context.Projects
                .Include(p => p.Issues) // Optionally include Issues related to this Project
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", id);
                return NotFound();
            }

            return project;
        }

        // GET: api/Project
        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            // Retrieve all projects from the database
            var projects = await _context.Projects.ToListAsync();
            return Ok(projects);
        }
    }
}
