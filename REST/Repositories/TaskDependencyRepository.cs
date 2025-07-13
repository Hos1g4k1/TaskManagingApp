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
    public class TaskDependencyRepository : ITaskDependencyRepository
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskDependencyRepository> _logger;

        public TaskDependencyRepository(Supabase.Client supabaseClient, ITaskRepository taskRepository, ILogger<TaskDependencyRepository> logger)
        {
            _supabaseClient = supabaseClient;
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<List<TaskDependency>> GetAllDependenciesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all task dependencies");

                var response = await _supabaseClient
                    .From<TaskDependency>()
                    .Get();

                var dependencies = response.Models;

                // Load related tasks for each dependency
                foreach (var dependency in dependencies)
                {
                    try
                    {
                        await LoadRelatedTasksSafely(dependency);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error loading related tasks for dependency {DependencyId}, continuing with partial data", dependency.DependencyId);
                    }
                }

                _logger.LogInformation("Fetched {Count} task dependencies", dependencies.Count);
                return dependencies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all task dependencies");
                throw;
            }
        }

        public async Task<TaskDependency> GetDependencyByIdAsync(long dependencyId)
        {
            try
            {
                _logger.LogInformation("Fetching task dependency with ID {DependencyId}", dependencyId);

                var response = await _supabaseClient
                    .From<TaskDependency>()
                    .Where(d => d.DependencyId == dependencyId)
                    .Get();

                if (response == null || response.Models.Count == 0)
                {
                    _logger.LogWarning("Dependency with ID {DependencyId} was not found", dependencyId);
                    throw new Exception($"Dependency with ID {dependencyId} not found");
                }

                var dependency = response.Models.First();

                // Load related tasks
                await LoadRelatedTasksSafely(dependency);

                _logger.LogInformation("Successfully fetched dependency with ID {DependencyId}", dependencyId);
                return dependency;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dependency with ID {DependencyId}", dependencyId);
                throw;
            }
        }

        public async Task<List<TaskDependency>> GetDependenciesForTaskAsync(long taskId)
        {
            try
            {
                _logger.LogInformation("Fetching dependencies for task ID {TaskId}", taskId);

                // Get dependencies where this task is the dependent (the task that depends on others)
                var response = await _supabaseClient
                    .From<TaskDependency>()
                    .Where(d => d.DependentTaskId == taskId)
                    .Get();

                if (response == null)
                {
                    _logger.LogWarning("No dependencies found for task ID {TaskId}", taskId);
                    return new List<TaskDependency>();
                }

                var dependencies = response.Models;

                // Load related tasks for each dependency
                foreach (var dependency in dependencies)
                {
                    try
                    {
                        await LoadRelatedTasksSafely(dependency);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error loading related tasks for dependency {DependencyId}, continuing with partial data", dependency.DependencyId);
                    }
                }

                _logger.LogInformation("Successfully fetched {Count} dependencies for task {TaskId}", dependencies.Count, taskId);
                return dependencies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dependencies for task {TaskId}", taskId);
                throw;
            }
        }

        public async Task<List<TaskDependency>> GetDependentsForTaskAsync(long taskId)
        {
            try
            {
                _logger.LogInformation("Fetching dependents for task ID {TaskId}", taskId);

                // Get dependencies where this task is the prerequisite (tasks that depend on this task)
                var response = await _supabaseClient
                    .From<TaskDependency>()
                    .Where(d => d.TaskId == taskId)
                    .Get();

                if (response == null)
                {
                    _logger.LogWarning("No dependents found for task ID {TaskId}", taskId);
                    return new List<TaskDependency>();
                }

                var dependencies = response.Models;

                // Load related tasks for each dependency
                foreach (var dependency in dependencies)
                {
                    try
                    {
                        await LoadRelatedTasksSafely(dependency);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error loading related tasks for dependency {DependencyId}, continuing with partial data", dependency.DependencyId);
                    }
                }

                _logger.LogInformation("Successfully fetched {Count} dependents for task {TaskId}", dependencies.Count, taskId);
                return dependencies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dependents for task {TaskId}", taskId);
                throw;
            }
        }

        public async Task<TaskDependency> AddDependencyAsync(TaskDependency dependency)
        {
            try
            {
                // Clear navigation properties before inserting
                var task = dependency.Task;
                var dependentTask = dependency.DependentTask;

                dependency.Task = null;
                dependency.DependentTask = null;

                var response = await _supabaseClient
                    .From<TaskDependency>()
                    .Insert(dependency);

                _logger.LogInformation("Added new dependency between tasks {TaskId} and {DependentTaskId}",
                    dependency.TaskId, dependency.DependentTaskId);

                var newDependency = response.Models.FirstOrDefault();

                if (newDependency != null)
                {
                    // Load related tasks for the new dependency
                    await LoadRelatedTasksSafely(newDependency);
                    return newDependency;
                }
                else
                {
                    // If no dependency was returned, return the original dependency
                    dependency.Task = task;
                    dependency.DependentTask = dependentTask;
                    return dependency;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding dependency between tasks {TaskId} and {DependentTaskId}",
                    dependency.TaskId, dependency.DependentTaskId);
                throw;
            }
        }

        public async Task<TaskDependency> UpdateDependencyAsync(TaskDependency dependency)
        {
            try
            {
                // Clear navigation properties before updating
                var task = dependency.Task;
                var dependentTask = dependency.DependentTask;

                dependency.Task = null;
                dependency.DependentTask = null;

                var response = await _supabaseClient
                    .From<TaskDependency>()
                    .Where(d => d.DependencyId == dependency.DependencyId)
                    .Update(dependency);

                _logger.LogInformation("Updated dependency with ID {DependencyId}", dependency.DependencyId);

                var updatedDependency = response.Models.FirstOrDefault();

                if (updatedDependency != null)
                {
                    // Load related tasks for the updated dependency
                    await LoadRelatedTasksSafely(updatedDependency);
                    return updatedDependency;
                }
                else
                {
                    // If no dependency was returned, return the original dependency
                    dependency.Task = task;
                    dependency.DependentTask = dependentTask;
                    return dependency;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dependency with ID {DependencyId}", dependency.DependencyId);
                throw;
            }
        }

        public async Task<bool> DeleteDependencyAsync(long dependencyId)
        {
            try
            {
                await _supabaseClient
                    .From<TaskDependency>()
                    .Where(d => d.DependencyId == dependencyId)
                    .Delete();

                _logger.LogInformation("Deleted dependency with ID {DependencyId}", dependencyId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting dependency with ID {DependencyId}", dependencyId);
                return false;
            }
        }

        public async Task<List<TaskDependencyDto>> GetAllDependencyDtosAsync()
        {
            var dependencies = await GetAllDependenciesAsync();
            return TaskDependencyDto.FromTaskDependencies(dependencies);
        }

        public async Task<TaskDependencyDto> GetDependencyDtoByIdAsync(long dependencyId)
        {
            var dependency = await GetDependencyByIdAsync(dependencyId);
            return TaskDependencyDto.FromTaskDependency(dependency);
        }

        public async Task<List<TaskDependencyDto>> GetDependencyDtosForTaskAsync(long taskId)
        {
            var dependencies = await GetDependenciesForTaskAsync(taskId);
            return TaskDependencyDto.FromTaskDependencies(dependencies);
        }

        public async Task<List<TaskDependencyDto>> GetDependentDtosForTaskAsync(long taskId)
        {
            var dependents = await GetDependentsForTaskAsync(taskId);
            return TaskDependencyDto.FromTaskDependencies(dependents);
        }

        // Helper method to safely load related tasks using TaskRepository
        private async System.Threading.Tasks.Task LoadRelatedTasksSafely(TaskDependency dependency)
        {
            // Load Task (the prerequisite task) using TaskRepository
            try
            {
                var task = await _taskRepository.GetTaskByIdAsync(dependency.TaskId);
                if (task != null)
                {
                    dependency.Task = task;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load Task {TaskId} for Dependency {DependencyId}",
                    dependency.TaskId, dependency.DependencyId);
            }

            // Load DependentTask (the task that depends on the prerequisite) using TaskRepository
            try
            {
                var dependentTask = await _taskRepository.GetTaskByIdAsync(dependency.DependentTaskId);
                if (dependentTask != null)
                {
                    dependency.DependentTask = dependentTask;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load DependentTask {DependentTaskId} for Dependency {DependencyId}",
                    dependency.DependentTaskId, dependency.DependencyId);
            }
        }
    }
}