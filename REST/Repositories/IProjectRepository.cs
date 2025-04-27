using REST.Models;
using REST.Models.DTOs;

namespace REST.Repositories
{
    public interface IProjectRepository
    {
        // Entity methods
        Task<List<Project>> GetAllProjectsAsync();
        Task<Project> GetProjectByIdAsync(long projectId);
        Task<Project> AddProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(long projectId);

        // DTO methods
        Task<List<ProjectDto>> GetAllProjectDtosAsync();
        Task<ProjectDto> GetProjectDtoByIdAsync(long projectId);
    }
} 