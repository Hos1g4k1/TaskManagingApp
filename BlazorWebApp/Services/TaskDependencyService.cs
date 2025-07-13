using BlazorWebApp.Models;
using BlazorWebApp.Models.DTOs;
using BlazorWebApp.Shared;

namespace BlazorWebApp.Services
{
    public class TaskDependencyService
    {
        private readonly HttpClientService _httpClientService;
        private readonly TaskService _taskService;

        public TaskDependencyService(HttpClientService httpClientService, TaskService taskService)
        {
            _httpClientService = httpClientService;
            _taskService = taskService;
        }

        public async Task<List<TaskDependency>> GetAllDependenciesAsync()
        {
            var dependencyDtos = await _httpClientService.GetAsync<List<TaskDependencyDto>>(ApiEndpoints.TaskDependencies);
            return dependencyDtos?.Select(dto => dto.ToTaskDependency()).ToList() ?? new List<TaskDependency>();
        }

        public async Task<TaskDependency?> GetDependencyByIdAsync(long dependencyId)
        {
            var dependencyDto = await _httpClientService.GetAsync<TaskDependencyDto>($"{ApiEndpoints.TaskDependencies}/{dependencyId}");
            return dependencyDto?.ToTaskDependency();
        }

        public async Task<List<TaskDependency>> GetDependenciesForTaskAsync(long taskId)
        {
            var dependencyDtos = await _httpClientService.GetAsync<List<TaskDependencyDto>>(ApiEndpoints.TaskDependenciesForTask(taskId));
            var dependencies = dependencyDtos?.Select(dto => dto.ToTaskDependency()).ToList() ?? new List<TaskDependency>();

            // Enrich dependencies with task status information
            foreach (var dependency in dependencies)
            {
                try
                {
                    var task = await _taskService.GetTaskByIdAsync(dependency.TaskId);
                    if (task != null)
                    {
                        dependency.TaskStatusName = task.StatusName;
                        dependency.TaskStatusId = task.StatusId;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading task {dependency.TaskId} for dependency: {ex.Message}");
                }
            }

            return dependencies;
        }

        public async Task<List<TaskDependency>> GetDependentsForTaskAsync(long taskId)
        {
            var dependencyDtos = await _httpClientService.GetAsync<List<TaskDependencyDto>>(ApiEndpoints.TaskDependentsForTask(taskId));
            return dependencyDtos?.Select(dto => dto.ToTaskDependency()).ToList() ?? new List<TaskDependency>();
        }

        public async Task<TaskDependency?> CreateDependencyAsync(TaskDependency dependency)
        {
            try
            {
                var dependencyDto = await _httpClientService.PostAsync<TaskDependencyDto>(ApiEndpoints.TaskDependencies, dependency);
                return dependencyDto?.ToTaskDependency();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating task dependency: {ex.Message}");
            }
        }

        public async Task<TaskDependency?> UpdateDependencyAsync(TaskDependency dependency)
        {
            try
            {
                var dependencyDto = await _httpClientService.PutAsync<TaskDependencyDto>($"{ApiEndpoints.TaskDependencies}/{dependency.DependencyId}", dependency);
                return dependencyDto?.ToTaskDependency();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating task dependency: {ex.Message}");
            }
        }

        public async Task<bool> DeleteDependencyAsync(long dependencyId)
        {
            try
            {
                await _httpClientService.DeleteAsync($"{ApiEndpoints.TaskDependencies}/{dependencyId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task dependency: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CanTaskBeCompletedAsync(long taskId)
        {
            try
            {
                var dependencies = await GetDependenciesForTaskAsync(taskId);

                // If there are no dependencies, task can be completed
                if (!dependencies.Any())
                    return true;

                // Check if all dependency tasks are completed
                foreach (var dependency in dependencies)
                {
                    if (dependency.TaskStatusName?.ToLower() != "completed")
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking task completion eligibility: {ex.Message}");
                return false;
            }
        }

        public async Task<List<TaskItem>> GetBlockedTasksAsync()
        {
            try
            {
                var allTasks = await _taskService.GetAllTasksAsync();
                var blockedTasks = new List<TaskItem>();

                foreach (var task in allTasks)
                {
                    if (!(await CanTaskBeCompletedAsync(task.TaskId)))
                    {
                        blockedTasks.Add(task);
                    }
                }

                return blockedTasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting blocked tasks: {ex.Message}");
                return new List<TaskItem>();
            }
        }
    }
}