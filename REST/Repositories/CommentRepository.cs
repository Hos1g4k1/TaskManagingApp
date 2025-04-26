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
    public class CommentRepository : ICommentRepository
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(Supabase.Client supabaseClient, ILogger<CommentRepository> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            var comments = await _supabaseClient
                .From<Comment>()
                .Get();

            _logger.LogInformation("Fetched {Count} comments from Supabase", comments.Models.Count);
            return comments.Models;
        }

        public async Task<Comment> GetCommentByIdAsync(long commentId)
        {
            try
            {
                var comment = await _supabaseClient
                    .From<Comment>()
                    .Where(c => c.CommentId == commentId)
                    .Single();

                _logger.LogInformation("Fetched comment with ID {CommentId}", commentId);
                return comment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching comment with ID {CommentId}", commentId);
                throw;
            }
        }

        public async Task<List<Comment>> GetCommentsByTaskIdAsync(long taskId)
        {
            var comments = await _supabaseClient
                .From<Comment>()
                .Where(c => c.TaskId == taskId)
                .Order(c => c.CreatedAt, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            _logger.LogInformation("Fetched {Count} comments for task ID {TaskId}",
                comments.Models.Count, taskId);
            return comments.Models;
        }

        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            // Set creation time if not already set
            if (comment.CreatedAt == default)
            {
                comment.CreatedAt = DateTime.UtcNow;
            }

            var response = await _supabaseClient
                .From<Comment>()
                .Insert(comment);

            _logger.LogInformation("Added new comment for task ID {TaskId}", comment.TaskId);
            return response.Models.FirstOrDefault();
        }

        public async Task<Comment> UpdateCommentAsync(Comment comment)
        {
            var response = await _supabaseClient
                .From<Comment>()
                .Where(c => c.CommentId == comment.CommentId)
                .Update(comment);

            _logger.LogInformation("Updated comment with ID {CommentId}", comment.CommentId);
            return response.Models.FirstOrDefault();
        }

        public async Task<bool> DeleteCommentAsync(long commentId)
        {
            try
            {
                await _supabaseClient
                    .From<Comment>()
                    .Where(c => c.CommentId == commentId)
                    .Delete();

                _logger.LogInformation("Deleted comment with ID {CommentId}", commentId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID {CommentId}", commentId);
                return false;
            }
        }

        public async Task<List<CommentDto>> GetAllCommentDtosAsync()
        {
            var comments = await GetAllCommentsAsync();
            var dtos = CommentDto.FromComments(comments);
            return dtos;
        }

        public async Task<CommentDto> GetCommentDtoByIdAsync(long commentId)
        {
            var comment = await GetCommentByIdAsync(commentId);
            return CommentDto.FromComment(comment);
        }

        public async Task<List<CommentDto>> GetCommentDtosByTaskIdAsync(long taskId)
        {
            var comments = await GetCommentsByTaskIdAsync(taskId);
            return CommentDto.FromComments(comments);
        }
    }
}