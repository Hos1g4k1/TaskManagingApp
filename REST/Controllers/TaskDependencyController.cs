using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using REST.Models;
using REST.Models.DTOs;
using REST.Repositories;

namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskDependencyController : ControllerBase
    {
        private readonly ITaskDependencyRepository _taskDependencyRepository;
        private readonly ILogger<TaskDependencyController> _logger;

        public TaskDependencyController(ITaskDependencyRepository taskDependencyRepository, ILogger<TaskDependencyController> logger)
        {
            _taskDependencyRepository = taskDependencyRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDependencies()
        {
            _logger.LogInformation("GetAllDependencies was called");
            var dependencyDtos = await _taskDependencyRepository.GetAllDependencyDtosAsync();
            return Ok(dependencyDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDependency(long id)
        {
            _logger.LogInformation("GetDependency was called with ID: {DependencyId}", id);
            try
            {
                var dependencyDto = await _taskDependencyRepository.GetDependencyDtoByIdAsync(id);
                return Ok(dependencyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dependency with ID: {DependencyId}. Error: {ErrorMessage}", id, ex.Message);
                
                // Check if it's a "not found" error
                if (ex.Message.Contains("not found"))
                {
                    return NotFound($"Dependency with ID {id} not found");
                }
                
                return StatusCode(500, $"Error retrieving dependency: {ex.Message}");
            }
        }

        [HttpGet("ForTask/{taskId}")]
        public async Task<IActionResult> GetDependenciesForTask(long taskId)
        {
            _logger.LogInformation("GetDependenciesForTask was called with Task ID: {TaskId}", taskId);
            try
            {
                var dependencyDtos = await _taskDependencyRepository.GetDependencyDtosForTaskAsync(taskId);
                return Ok(dependencyDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dependencies for task with ID: {TaskId}", taskId);
                return StatusCode(500, $"Error retrieving dependencies: {ex.Message}");
            }
        }

        [HttpGet("DependentOn/{taskId}")]
        public async Task<IActionResult> GetDependentsForTask(long taskId)
        {
            _logger.LogInformation("GetDependentsForTask was called with Task ID: {TaskId}", taskId);
            try
            {
                var dependencyDtos = await _taskDependencyRepository.GetDependentDtosForTaskAsync(taskId);
                return Ok(dependencyDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dependents for task with ID: {TaskId}", taskId);
                return StatusCode(500, $"Error retrieving dependent tasks: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddDependency(TaskDependency dependency)
        {
            _logger.LogInformation("AddDependency was called between tasks {TaskId} and {DependentTaskId}", 
                dependency.TaskId, dependency.DependentTaskId);

            if (dependency == null)
            {
                return BadRequest("Dependency data is required");
            }

            try
            {
                var newDependency = await _taskDependencyRepository.AddDependencyAsync(dependency);
                var dependencyDto = TaskDependencyDto.FromTaskDependency(newDependency);
                return CreatedAtAction(nameof(GetDependency), new { id = dependencyDto.DependencyId }, dependencyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding dependency between tasks {TaskId} and {DependentTaskId}", 
                    dependency.TaskId, dependency.DependentTaskId);
                return StatusCode(500, $"Error creating dependency: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDependency(long id, TaskDependency dependency)
        {
            _logger.LogInformation("UpdateDependency was called for ID: {DependencyId}", id);

            if (dependency == null || id != dependency.DependencyId)
            {
                return BadRequest("Dependency data is invalid");
            }

            try
            {
                var updatedDependency = await _taskDependencyRepository.UpdateDependencyAsync(dependency);
                var dependencyDto = TaskDependencyDto.FromTaskDependency(updatedDependency);
                return Ok(dependencyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dependency with ID: {DependencyId}", id);
                
                if (ex.Message.Contains("not found"))
                {
                    return NotFound($"Dependency with ID {id} not found");
                }
                
                return StatusCode(500, $"Error updating dependency: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDependency(long id)
        {
            _logger.LogInformation("DeleteDependency was called for ID: {DependencyId}", id);

            var result = await _taskDependencyRepository.DeleteDependencyAsync(id);
            if (result)
            {
                return NoContent();
            }

            return NotFound($"Dependency with ID {id} not found");
        }
    }
} 