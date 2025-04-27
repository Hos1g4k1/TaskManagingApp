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
    public class ProjectRepository : IProjectRepository
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(Supabase.Client supabaseClient, ILogger<ProjectRepository> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            var projects = await _supabaseClient
                .From<Project>()
                .Get();

            // Load statuses for all projects - handle each project individually
            foreach (var project in projects.Models)
            {
                try {
                    // Only try to load status if StatusId is not null
                    if (project.StatusId.HasValue)
                    {
                        var status = await _supabaseClient
                            .From<Status>()
                            .Where(s => s.StatusId == project.StatusId.Value)
                            .Single();
                        
                        project.Status = status;
                    }
                }
                catch (Exception ex) {
                    _logger.LogWarning(ex, "Could not load Status for Project {ProjectId}", project.ProjectId);
                    // Continue processing other projects even if this one fails
                }
            }

            _logger.LogInformation("Fetched {Count} projects from Supabase", projects.Models.Count);
            return projects.Models;
        }

        public async Task<Project> GetProjectByIdAsync(long projectId)
        {
            try
            {
                var project = await _supabaseClient
                    .From<Project>()
                    .Where(p => p.ProjectId == projectId)
                    .Single();

                // Load the associated Status
                try {
                    if (project.StatusId.HasValue)
                    {
                        var status = await _supabaseClient
                            .From<Status>()
                            .Where(s => s.StatusId == project.StatusId.Value)
                            .Single();
                        
                        project.Status = status;
                    }
                }
                catch (Exception ex) {
                    _logger.LogWarning(ex, "Could not load Status for Project {ProjectId}", projectId);
                }

                _logger.LogInformation("Fetched project with ID {ProjectId}", projectId);
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching project with ID {ProjectId}", projectId);
                throw;
            }
        }

       public async Task<Project> AddProjectAsync(Project project)
        {
            var response = await _supabaseClient
                .From<Project>()
                .Insert(project);
            
            _logger.LogInformation("Added new project with name {ProjectName}", project.Name);
            var newProject = response.Models.FirstOrDefault();
            
            return newProject;
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            var response = await _supabaseClient
                .From<Project>()
                .Where(p => p.ProjectId == project.ProjectId)
                .Update(project);

            _logger.LogInformation("Updated project with ID {ProjectId}", project.ProjectId);
            return response.Models.FirstOrDefault();
        }

        public async Task<bool> DeleteProjectAsync(long projectId)
        {
            try
            {
                await _supabaseClient
                    .From<Project>()
                    .Where(p => p.ProjectId == projectId)
                    .Delete();

                _logger.LogInformation("Deleted project with ID {ProjectId}", projectId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID {ProjectId}", projectId);
                return false;
            }
        }

        public async Task<List<ProjectDto>> GetAllProjectDtosAsync()
        {
            var projects = await GetAllProjectsAsync();
            var dtos = ProjectDto.FromProjects(projects);
            return dtos;
        }

        public async Task<ProjectDto> GetProjectDtoByIdAsync(long projectId)
        {
            var project = await GetProjectByIdAsync(projectId);
            return ProjectDto.FromProject(project);
        }
    }
} 