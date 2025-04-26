using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using REST.Models;
using REST.Models.DTOs;
using REST.Repositories;
using System;
using System.Threading.Tasks;

namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusRepository _statusRepository;
        private readonly ILogger<StatusController> _logger;

        public StatusController(IStatusRepository statusRepository, ILogger<StatusController> logger)
        {
            _statusRepository = statusRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStatuses()
        {
            _logger.LogInformation("GetAllStatuses was called");
            var statusDtos = await _statusRepository.GetAllStatusDtosAsync();
            return Ok(statusDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatus(long id)
        {
            _logger.LogInformation("GetStatus was called with ID: {StatusId}", id);
            try
            {
                var statusDto = await _statusRepository.GetStatusDtoByIdAsync(id);
                return Ok(statusDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching status with ID: {StatusId}", id);
                return NotFound($"Status with ID {id} not found");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddStatus(Status status)
        {
            _logger.LogInformation("AddStatus was called with name: {StatusName}", status.Name);

            if (status == null)
            {
                return BadRequest("Status data is required");
            }

            var newStatus = await _statusRepository.AddStatusAsync(status);
            var statusDto = StatusDto.FromStatus(newStatus);
            return CreatedAtAction(nameof(GetStatus), new { id = statusDto.StatusId }, statusDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(long id, Status status)
        {
            _logger.LogInformation("UpdateStatus was called for ID: {StatusId}", id);

            if (status == null || id != status.StatusId)
            {
                return BadRequest("Status data is invalid");
            }

            try
            {
                var updatedStatus = await _statusRepository.UpdateStatusAsync(status);
                var statusDto = StatusDto.FromStatus(updatedStatus);
                return Ok(statusDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status with ID: {StatusId}", id);
                return NotFound($"Status with ID {id} not found");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(long id)
        {
            _logger.LogInformation("DeleteStatus was called for ID: {StatusId}", id);

            var result = await _statusRepository.DeleteStatusAsync(id);
            if (result)
            {
                return NoContent();
            }

            return NotFound($"Status with ID {id} not found");
        }
    }
}
