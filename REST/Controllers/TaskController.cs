using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using REST.Models;
using REST.Models.DTOs;
using REST.Repositories;

namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskRepository taskRepository, ILogger<TaskController> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            _logger.LogInformation("GetAllTasks was called");
            var taskDtos = await _taskRepository.GetAllTaskDtosAsync();
            return Ok(taskDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(long id)
        {
            _logger.LogInformation("GetTask was called with ID: {TaskId}", id);
            try
            {
                var taskDto = await _taskRepository.GetTaskDtoByIdAsync(id);
                return Ok(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching task with ID: {TaskId}. Error: {ErrorMessage}", id, ex.Message);
                
                // Check if it's a "not found" error
                if (ex.Message.Contains("Not Found") || ex.Message.Contains("404"))
                {
                    return NotFound($"Task with ID {id} not found");
                }
                
                // Return a more detailed error
                return StatusCode(500, $"Error retrieving task: {ex.Message}");
            }
        }

        [HttpGet("Project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(long projectId)
        {
            _logger.LogInformation("GetTasksByProject was called with Project ID: {ProjectId}", projectId);
            try
            {
                var taskDtos = await _taskRepository.GetTaskDtosByProjectAsync(projectId);
                return Ok(taskDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tasks for project with ID: {ProjectId}", projectId);
                return NotFound($"Tasks for project with ID {projectId} not found");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(Models.Task task)
        {
            _logger.LogInformation("AddTask was called with Title: {TaskTitle}", task.Title);

            if (task == null)
            {
                return BadRequest("Task data is required");
            }

            var newTask = await _taskRepository.AddTaskAsync(task);
            var taskDto = TaskDto.FromTask(newTask);
            return CreatedAtAction(nameof(GetTask), new { id = taskDto.TaskId }, taskDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(long id, Models.Task task)
        {
            _logger.LogInformation("UpdateTask was called for ID: {TaskId}", id);

            if (task == null || id != task.TaskId)
            {
                return BadRequest("Task data is invalid");
            }

            try
            {
                var updatedTask = await _taskRepository.UpdateTaskAsync(task);
                var taskDto = TaskDto.FromTask(updatedTask);
                return Ok(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {TaskId}", id);
                return NotFound($"Task with ID {id} not found");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            _logger.LogInformation("DeleteTask was called for ID: {TaskId}", id);

            var result = await _taskRepository.DeleteTaskAsync(id);
            if (result)
            {
                return NoContent();
            }

            return NotFound($"Task with ID {id} not found");
        }

        [HttpGet("Raw/5")]
        public async Task<IActionResult> GetRawTask5()
        {
            _logger.LogInformation("GetRawTask5 was called to directly fetch task ID 5");
            try
            {
                var client = (_taskRepository as TaskRepository)._supabaseClient;
                var response = await client.From<Models.Task>()
                    .Where(t => t.TaskId == 5)
                    .Get();
                    
                if (response == null || response.Models.Count == 0)
                {
                    _logger.LogWarning("Task with ID 5 was not found in database");
                    return NotFound("Task 5 not found");
                }
                
                // Return the raw task object
                var task = response.Models.First();
                
                // Convert to simple anonymous object to avoid serialization issues
                var simpleTask = new
                {
                    task_id = task.TaskId,
                    title = task.Title,
                    description = task.Description,
                    project_id = task.ProjectId,
                    status_id = task.StatusId,
                    due_date = task.DueDate,
                    created_at = task.CreatedAt
                };
                
                _logger.LogInformation("Successfully fetched raw task with ID 5");
                return Ok(simpleTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching raw task 5: {ErrorMessage}", ex.Message);
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
} 