using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using REST.Models;
using REST.Models.DTOs;
using REST.Repositories;

namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IProjectRepository projectRepository, ILogger<ProjectController> logger)
        {
            _projectRepository = projectRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            _logger.LogInformation("GetAllProjects was called");
            var projectDtos = await _projectRepository.GetAllProjectDtosAsync();
            return Ok(projectDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(long id)
        {
            _logger.LogInformation("GetProject was called with ID: {ProjectId}", id);
            try
            {
                var projectDto = await _projectRepository.GetProjectDtoByIdAsync(id);
                return Ok(projectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching project with ID: {ProjectId}", id);
                return NotFound($"Project with ID {id} not found");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(Project project)
        {
            _logger.LogInformation("AddProject was called with Name: {ProjectName}", project.Name);

            if (project == null)
            {
                return BadRequest("Project data is required");
            }

            var newProject = await _projectRepository.AddProjectAsync(project);
            var projectDto = ProjectDto.FromProject(newProject);
            return CreatedAtAction(nameof(GetProject), new { id = projectDto.ProjectId }, projectDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(long id, Project project)
        {
            _logger.LogInformation("UpdateProject was called for ID: {ProjectId}", id);

            if (project == null || id != project.ProjectId)
            {
                return BadRequest("Project data is invalid");
            }

            try
            {
                var updatedProject = await _projectRepository.UpdateProjectAsync(project);
                var projectDto = ProjectDto.FromProject(updatedProject);
                return Ok(projectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with ID: {ProjectId}", id);
                return NotFound($"Project with ID {id} not found");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(long id)
        {
            _logger.LogInformation("DeleteProject was called for ID: {ProjectId}", id);

            var result = await _projectRepository.DeleteProjectAsync(id);
            if (result)
            {
                return NoContent();
            }

            return NotFound($"Project with ID {id} not found");
        }
    }
} 