using REST.Models;
using REST.Models.DTOs;

namespace REST.Repositories
{
    public interface ITaskDependencyRepository
    {
        // Entity methods
        Task<List<TaskDependency>> GetAllDependenciesAsync();
        Task<TaskDependency> GetDependencyByIdAsync(long dependencyId);
        Task<List<TaskDependency>> GetDependenciesForTaskAsync(long taskId);
        Task<List<TaskDependency>> GetDependentsForTaskAsync(long taskId);
        Task<TaskDependency> AddDependencyAsync(TaskDependency dependency);
        Task<TaskDependency> UpdateDependencyAsync(TaskDependency dependency);
        Task<bool> DeleteDependencyAsync(long dependencyId);
        
        // DTO methods
        Task<List<TaskDependencyDto>> GetAllDependencyDtosAsync();
        Task<TaskDependencyDto> GetDependencyDtoByIdAsync(long dependencyId);
        Task<List<TaskDependencyDto>> GetDependencyDtosForTaskAsync(long taskId);
        Task<List<TaskDependencyDto>> GetDependentDtosForTaskAsync(long taskId);
    }
} 