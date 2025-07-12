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
                if (ex.Message.Contains("Not Found") || ex.Message.Contains("404") || ex.Message.Contains("not found"))
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

            if (task == null)
            {
                return BadRequest("Task data is required");
            }

            try
            {
                var newTask = await _taskRepository.AddTaskAsync(task);

                var taskDto = TaskDto.FromTask(newTask);

                // Return 201 Created with the task data directly instead of using CreatedAtAction
                // This avoids the immediate GetTask call that fails due to read-after-write consistency issues
                return Created($"/api/Task/{taskDto.TaskId}", taskDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating task: {ex.Message}");
            }
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
    }
}