using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using REST.Models;
using REST.Models.DTOs;
using REST.Repositories;

namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentRepository commentRepository, ILogger<CommentController> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            _logger.LogInformation("GetAllComments was called");
            var commentDtos = await _commentRepository.GetAllCommentDtosAsync();
            return Ok(commentDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(long id)
        {
            _logger.LogInformation("GetComment was called with ID: {CommentId}", id);
            try
            {
                var commentDto = await _commentRepository.GetCommentDtoByIdAsync(id);
                return Ok(commentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching comment with ID: {CommentId}", id);
                return NotFound($"Comment with ID {id} not found");
            }
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetCommentsByTask(long taskId)
        {
            _logger.LogInformation("GetCommentsByTask was called with TaskID: {TaskId}", taskId);
            var commentDtos = await _commentRepository.GetCommentDtosByTaskIdAsync(taskId);
            return Ok(commentDtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            _logger.LogInformation("AddComment was called for TaskID: {TaskId}", comment.TaskId);

            if (comment == null)
            {
                return BadRequest("Comment data is required");
            }

            var newComment = await _commentRepository.AddCommentAsync(comment);
            var commentDto = CommentDto.FromComment(newComment);
            return CreatedAtAction(nameof(GetComment), new { id = commentDto.CommentId }, commentDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(long id, Comment comment)
        {
            _logger.LogInformation("UpdateComment was called for ID: {CommentId}", id);

            if (comment == null || id != comment.CommentId)
            {
                return BadRequest("Comment data is invalid");
            }

            try
            {
                var updatedComment = await _commentRepository.UpdateCommentAsync(comment);
                var commentDto = CommentDto.FromComment(updatedComment);
                return Ok(commentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment with ID: {CommentId}", id);
                return NotFound($"Comment with ID {id} not found");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            _logger.LogInformation("DeleteComment was called for ID: {CommentId}", id);

            var result = await _commentRepository.DeleteCommentAsync(id);
            if (result)
            {
                return NoContent();
            }

            return NotFound($"Comment with ID {id} not found");
        }
    }
}