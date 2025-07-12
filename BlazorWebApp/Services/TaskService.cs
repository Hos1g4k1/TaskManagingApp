using BlazorWebApp.Models;
using BlazorWebApp.Models.DTOs;
using BlazorWebApp.Shared;

namespace BlazorWebApp.Services
{
    public class TaskService
    {
        private readonly HttpClientService _httpClientService;

        public TaskService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            var taskDtos = await _httpClientService.GetAsync<List<TaskDto>>(ApiEndpoints.Tasks);
            return taskDtos?.Select(dto => dto.ToTask()).ToList() ?? new List<TaskItem>();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(long id)
        {
            var taskDto = await _httpClientService.GetAsync<TaskDto>($"{ApiEndpoints.Tasks}/{id}");
            return taskDto?.ToTask();
        }

        public async Task<List<TaskItem>> GetTasksByProjectAsync(long projectId)
        {
            var taskDtos = await _httpClientService.GetAsync<List<TaskDto>>(ApiEndpoints.TasksByProject(projectId));
            return taskDtos?.Select(dto => dto.ToTask()).ToList() ?? new List<TaskItem>();
        }

        public async Task<TaskItem?> CreateTaskAsync(TaskItem task)
        {
            try
            {
                var taskDto = await _httpClientService.PostAsync<TaskDto>(ApiEndpoints.Tasks, task);

                if (taskDto != null)
                {
                    var result = taskDto.ToTask();
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TaskItem?> UpdateTaskAsync(TaskItem task)
        {
            var taskDto = await _httpClientService.PutAsync<TaskDto>($"{ApiEndpoints.Tasks}/{task.TaskId}", task);
            return taskDto?.ToTask();
        }

        public async System.Threading.Tasks.Task DeleteTaskAsync(long id)
        {
            await _httpClientService.DeleteAsync($"{ApiEndpoints.Tasks}/{id}");
        }
    }
}