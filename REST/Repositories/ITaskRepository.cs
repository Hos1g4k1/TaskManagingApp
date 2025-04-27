using REST.Models;
using REST.Models.DTOs;

namespace REST.Repositories
{
    public interface ITaskRepository
    {
        // Entity methods
        Task<List<Models.Task>> GetAllTasksAsync();
        Task<Models.Task> GetTaskByIdAsync(long taskId);
        Task<List<Models.Task>> GetTasksByProjectAsync(long projectId);
        Task<Models.Task> AddTaskAsync(Models.Task task);
        Task<Models.Task> UpdateTaskAsync(Models.Task task);
        Task<bool> DeleteTaskAsync(long taskId);

        // DTO methods
        Task<List<TaskDto>> GetAllTaskDtosAsync();
        Task<TaskDto> GetTaskDtoByIdAsync(long taskId);
        Task<List<TaskDto>> GetTaskDtosByProjectAsync(long projectId);
    }
} 