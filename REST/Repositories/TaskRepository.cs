using Supabase;
using Supabase.Postgrest;
using REST.Models;
using REST.Models.DTOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace REST.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        public readonly Supabase.Client _supabaseClient;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(Supabase.Client supabaseClient, ILogger<TaskRepository> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        public async Task<List<Models.Task>> GetAllTasksAsync()
        {
            var response = await _supabaseClient
                .From<Models.Task>()
                .Get();

            var tasks = response.Models;

            // Load related entities for each task
            foreach (var task in tasks)
            {
                try
                {
                    await LoadRelatedEntitiesSafely(task);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error loading related entities for task {TaskId}, continuing with partial data", task.TaskId);

                    // Ensure task has at least basic collections initialized
                    if (task.Comments == null)
                    {
                        task.Comments = new List<Comment>();
                    }
                }
            }

            _logger.LogInformation("Fetched {Count} tasks from Supabase", tasks.Count);
            return tasks;
        }

        public async Task<Models.Task> GetTaskByIdAsync(long taskId)
        {
            try
            {
                _logger.LogInformation("Attempting to fetch task with ID {TaskId}", taskId);

                // Try up to 3 times with increasing delays to handle read-after-write consistency issues
                for (int attempt = 1; attempt <= 3; attempt++)
                {
                    var response = await _supabaseClient
                        .From<Models.Task>()
                        .Where(t => t.TaskId == taskId)
                        .Get();

                    if (response != null && response.Models.Count > 0)
                    {
                        var task = response.Models.First();

                        // Load related entities safely
                        try
                        {
                            await LoadRelatedEntitiesSafely(task);
                        }
                        catch (Exception ex)
                        {
                            // Ensure task has at least basic collections initialized
                            if (task.Comments == null)
                            {
                                task.Comments = new List<Comment>();
                            }
                        }

                        return task;
                    }

                    if (attempt < 3)
                    {
                        var delayMs = attempt * 200; // 200ms, 400ms delays
                        await System.Threading.Tasks.Task.Delay(delayMs);
                    }
                }

                throw new Exception($"Task with ID {taskId} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching task with ID {TaskId}. Error details: {ErrorMessage}", taskId, ex.ToString());
                throw;
            }
        }

        public async Task<Models.Task> GetTaskByIdDirectAsync(long taskId)
        {
            try
            {
                _logger.LogInformation("Direct fetch attempt for task with ID {TaskId}", taskId);

                // Simple direct query without loading relationships
                var task = await _supabaseClient
                    .From<Models.Task>()
                    .Where(t => t.TaskId == taskId)
                    .Single();

                _logger.LogInformation("Successfully fetched task directly with ID {TaskId}", taskId);
                return task;
            }
            catch (Exception ex)
            {
                    taskId, ex.ToString());
                throw;
            }
        }

        public async Task<List<Models.Task>> GetTasksByProjectAsync(long projectId)
        {
            try
            {
                _logger.LogInformation("Fetching tasks for project with ID {ProjectId}", projectId);

                var response = await _supabaseClient
                    .From<Models.Task>()
                    .Where(t => t.ProjectId == projectId)
                    .Get();

                if (response == null)
                {
                    _logger.LogWarning("No tasks found for project with ID {ProjectId}", projectId);
                    return new List<Models.Task>();
                }

                var tasks = response.Models;

                // Load related entities for each task
                foreach (var task in tasks)
                {
                    try
                    {
                        await LoadRelatedEntitiesSafely(task);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error loading related entities for task {TaskId}, continuing with partial data", task.TaskId);

                        // Ensure task has at least basic collections initialized
                        if (task.Comments == null)
                        {
                            task.Comments = new List<Comment>();
                        }
                    }
                }

                _logger.LogInformation("Successfully fetched {Count} tasks for project {ProjectId}", tasks.Count, projectId);
                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tasks for project {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<Models.Task> AddTaskAsync(Models.Task task)
        {
            try
            {

                // SIMPLIFIED: Use the same approach as Project (which works)
                var response = await _supabaseClient
                    .From<Models.Task>()
                    .Insert(task);


                var newTask = response.Models.FirstOrDefault();

                if (newTask != null)
                {

                    return newTask;
                }
                else
                {
                    return task;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Models.Task> UpdateTaskAsync(Models.Task task)
        {
            try
            {
                // Clear navigation properties before updating
                var project = task.Project;
                var status = task.Status;
                var comments = task.Comments;

                task.Project = null;
                task.Status = null;
                task.Comments = null;

                var response = await _supabaseClient
                    .From<Models.Task>()
                    .Where(t => t.TaskId == task.TaskId)
                    .Update(task);

                _logger.LogInformation("Updated task with ID {TaskId}", task.TaskId);
                var updatedTask = response.Models.FirstOrDefault();

                if (updatedTask != null)
                {
                    // Restore navigation properties if needed
                    updatedTask.Project = project;
                    updatedTask.Status = status;
                    updatedTask.Comments = comments;

                    return updatedTask;
                }
                else
                {
                    // If no task was returned, return the original task
                    task.Project = project;
                    task.Status = status;
                    task.Comments = comments;
                    return task;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID {TaskId}", task.TaskId);
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(long taskId)
        {
            try
            {
                await _supabaseClient
                    .From<Models.Task>()
                    .Where(t => t.TaskId == taskId)
                    .Delete();

                _logger.LogInformation("Deleted task with ID {TaskId}", taskId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID {TaskId}", taskId);
                return false;
            }
        }

        public async Task<List<TaskDto>> GetAllTaskDtosAsync()
        {
            var tasks = await GetAllTasksAsync();
            var dtos = TaskDto.FromTasks(tasks);
            return dtos;
        }

        public async Task<TaskDto> GetTaskDtoByIdAsync(long taskId)
        {
            var task = await GetTaskByIdAsync(taskId);
            return TaskDto.FromTask(task);
        }

        public async Task<List<TaskDto>> GetTaskDtosByProjectAsync(long projectId)
        {
            var tasks = await GetTasksByProjectAsync(projectId);
            var dtos = TaskDto.FromTasks(tasks);
            return dtos;
        }

        // Helper method to safely load related entities
        private async System.Threading.Tasks.Task LoadRelatedEntitiesSafely(Models.Task task)
        {
            // Always initialize Collections to prevent null reference issues
            task.Comments = new List<Comment>();

            // Load Status if StatusId has value
            if (task.StatusId.HasValue)
            {
                try
                {
                    var status = await _supabaseClient
                        .From<Status>()
                        .Where(s => s.StatusId == task.StatusId.Value)
                        .Get();

                    if (status != null && status.Models.Count > 0)
                    {
                        task.Status = status.Models.First();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not load Status for Task {TaskId}", task.TaskId);
                }
            }

            // Load Project
            try
            {
                var project = await _supabaseClient
                    .From<Project>()
                    .Where(p => p.ProjectId == task.ProjectId)
                    .Get();

                if (project != null && project.Models.Count > 0)
                {
                    task.Project = project.Models.First();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load Project for Task {TaskId}", task.TaskId);
            }

            // Load Comments - Make this completely safe and optional
            try
            {
                var comments = await _supabaseClient
                    .From<Comment>()
                    .Where(c => c.TaskId == task.TaskId)
                    .Get();

                if (comments != null && comments.Models != null)
                {
                    task.Comments = comments.Models;
                }
                else
                {
                    task.Comments = new List<Comment>(); // Ensure it's never null
                }
            }
            catch (Exception ex)
            {
                task.Comments = new List<Comment>(); // Ensure it's never null even on error
            }
        }

        // Add this method to get tasks using a raw SQL query
        public async Task<Models.Task> GetTaskBySqlAsync(long taskId)
        {
            try
            {
                _logger.LogInformation("Getting task by SQL for ID {TaskId}", taskId);

                // Use raw query to bypass Supabase ORM issues
                var result = await _supabaseClient.Postgrest.Rpc("get_task_by_id", new Dictionary<string, object>
                {
                    { "p_task_id", taskId }
                });

                if (string.IsNullOrEmpty(result?.Content) || result.Content == "null")
                {
                    throw new Exception($"Task with ID {taskId} not found");
                }

                // Parse the JSON result manually
                var taskData = System.Text.Json.JsonDocument.Parse(result.Content).RootElement;

                var task = new Models.Task
                {
                    TaskId = taskData.GetProperty("task_id").GetInt64(),
                    Title = taskData.GetProperty("title").GetString(),
                    ProjectId = taskData.GetProperty("project_id").GetInt64(),
                    StatusId = taskData.TryGetProperty("status_id", out var statusIdProp) && !statusIdProp.ValueKind.Equals(System.Text.Json.JsonValueKind.Null)
                        ? statusIdProp.GetInt64() : null,
                    Description = taskData.TryGetProperty("description", out var descProp) && !descProp.ValueKind.Equals(System.Text.Json.JsonValueKind.Null)
                        ? descProp.GetString() : null,
                    DueDate = taskData.TryGetProperty("due_date", out var dueDateProp) && !dueDateProp.ValueKind.Equals(System.Text.Json.JsonValueKind.Null)
                        ? DateTime.Parse(dueDateProp.GetString()) : null,
                    CreatedAt = taskData.TryGetProperty("created_at", out var createdAtProp) && !createdAtProp.ValueKind.Equals(System.Text.Json.JsonValueKind.Null)
                        ? DateTime.Parse(createdAtProp.GetString()) : DateTime.UtcNow
                };

                // Load related entities separately
                try
                {
                    await LoadRelatedEntitiesSafely(task);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error loading related entities for task {TaskId}, continuing with partial data", taskId);
                }

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task by SQL for ID {TaskId}: {ErrorMessage}", taskId, ex.Message);
                throw;
            }
        }
    }
}