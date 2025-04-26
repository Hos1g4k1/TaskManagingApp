using Supabase;
using REST.Models;
using REST.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace REST.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ILogger<StatusRepository> _logger;

        public StatusRepository(Supabase.Client supabaseClient, ILogger<StatusRepository> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        // Entity methods
        public async Task<List<Status>> GetAllStatusesAsync()
        {
            var statuses = await _supabaseClient
                .From<Models.Status>()
                .Get();

            _logger.LogInformation("Fetched {Count} statuses from Supabase", statuses.Models.Count);
            return statuses.Models;
        }

        public async Task<Status> GetStatusByIdAsync(long statusId)
        {
            try
            {
                var status = await _supabaseClient
                    .From<Status>()
                    .Where(s => s.StatusId == statusId)
                    .Single();

                _logger.LogInformation("Fetched status with ID {StatusId}", statusId);
                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching status with ID {StatusId}", statusId);
                throw;
            }
        }

        public async Task<Status> AddStatusAsync(Status status)
        {
            var response = await _supabaseClient
                .From<Status>()
                .Insert(status);

            _logger.LogInformation("Added new status with name {StatusName}", status.Name);
            return response.Models.FirstOrDefault();
        }

        public async Task<Status> UpdateStatusAsync(Status status)
        {
            var response = await _supabaseClient
                .From<Status>()
                .Where(s => s.StatusId == status.StatusId)
                .Update(status);

            _logger.LogInformation("Updated status with ID {StatusId}", status.StatusId);
            return response.Models.FirstOrDefault();
        }

        public async Task<bool> DeleteStatusAsync(long statusId)
        {
            try
            {
                await _supabaseClient
                    .From<Status>()
                    .Where(s => s.StatusId == statusId)
                    .Delete();

                _logger.LogInformation("Deleted status with ID {StatusId}", statusId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting status with ID {StatusId}", statusId);
                return false;
            }
        }

        // DTO methods
        public async Task<List<StatusDto>> GetAllStatusDtosAsync()
        {
            var statuses = await GetAllStatusesAsync();
            var dtos = StatusDto.FromStatuses(statuses);
            return dtos;
        }

        public async Task<StatusDto> GetStatusDtoByIdAsync(long statusId)
        {
            var status = await GetStatusByIdAsync(statusId);
            return StatusDto.FromStatus(status);
        }
    }
}