using REST.Models;
using REST.Models.DTOs;

namespace REST.Repositories
{
    public interface IStatusRepository
    {
        // Entity methods
        Task<List<Status>> GetAllStatusesAsync();
        Task<Status> GetStatusByIdAsync(long statusId);
        Task<Status> AddStatusAsync(Status status);
        Task<Status> UpdateStatusAsync(Status status);
        Task<bool> DeleteStatusAsync(long statusId);

        // DTO methods
        Task<List<StatusDto>> GetAllStatusDtosAsync();
        Task<StatusDto> GetStatusDtoByIdAsync(long statusId);
    }
}