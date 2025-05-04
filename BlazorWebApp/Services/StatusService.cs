using BlazorWebApp.Models;
using BlazorWebApp.Models.DTOs;
using BlazorWebApp.Shared;

namespace BlazorWebApp.Services
{
    public class StatusService
    {
        private readonly HttpClientService _httpClientService;

        public StatusService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<List<Status>> GetAllStatusesAsync()
        {
            var statusDtos = await _httpClientService.GetAsync<List<StatusDto>>(ApiEndpoints.Statuses);
            return statusDtos?.Select(dto => dto.ToStatus()).ToList() ?? new List<Status>();
        }

        public async Task<Status?> GetStatusByIdAsync(long id)
        {
            var statusDto = await _httpClientService.GetAsync<StatusDto>($"{ApiEndpoints.Statuses}/{id}");
            return statusDto?.ToStatus();
        }
    }
}