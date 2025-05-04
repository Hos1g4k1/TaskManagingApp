using BlazorWebApp.Models;
using BlazorWebApp.Models.DTOs;
using BlazorWebApp.Shared;

namespace BlazorWebApp.Services
{
    public class ProjectService
    {
        private readonly HttpClientService _httpClientService;

        public ProjectService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            var projectDtos = await _httpClientService.GetAsync<List<ProjectDto>>(ApiEndpoints.Projects);
            return projectDtos?.Select(dto => dto.ToProject()).ToList() ?? new List<Project>();
        }

        public async Task<Project?> GetProjectByIdAsync(long id)
        {
            var projectDto = await _httpClientService.GetAsync<ProjectDto>($"{ApiEndpoints.Projects}/{id}");
            return projectDto?.ToProject();
        }

        public async Task<Project?> CreateProjectAsync(Project project)
        {
            var projectDto = await _httpClientService.PostAsync<ProjectDto>(ApiEndpoints.Projects, project);
            return projectDto?.ToProject();
        }

        public async Task<Project?> UpdateProjectAsync(Project project)
        {
            var projectDto = await _httpClientService.PutAsync<ProjectDto>($"{ApiEndpoints.Projects}/{project.ProjectId}", project);
            return projectDto?.ToProject();
        }

        public async Task DeleteProjectAsync(long id)
        {
            await _httpClientService.DeleteAsync($"{ApiEndpoints.Projects}/{id}");
        }
    }
}