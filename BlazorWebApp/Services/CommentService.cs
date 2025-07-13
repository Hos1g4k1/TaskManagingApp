using BlazorWebApp.Models;
using BlazorWebApp.Models.DTOs;
using BlazorWebApp.Shared;

namespace BlazorWebApp.Services
{
    public class CommentService
    {
        private readonly HttpClientService _httpClientService;

        public CommentService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            var commentDtos = await _httpClientService.GetAsync<List<CommentDto>>(ApiEndpoints.Comments);
            return commentDtos?.Select(dto => dto.ToComment()).ToList() ?? new List<Comment>();
        }

        public async Task<Comment?> GetCommentByIdAsync(long id)
        {
            var commentDto = await _httpClientService.GetAsync<CommentDto>($"{ApiEndpoints.Comments}/{id}");
            return commentDto?.ToComment();
        }

        public async Task<List<Comment>> GetCommentsByTaskAsync(long taskId)
        {
            var commentDtos = await _httpClientService.GetAsync<List<CommentDto>>(ApiEndpoints.CommentsByTask(taskId));
            return commentDtos?.Select(dto => dto.ToComment()).ToList() ?? new List<Comment>();
        }

        public async Task<Comment?> CreateCommentAsync(Comment comment)
        {
            try
            {
                var commentDto = await _httpClientService.PostAsync<CommentDto>(ApiEndpoints.Comments, comment);
                return commentDto?.ToComment();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating comment: {ex.Message}");
                return null;
            }
        }

        public async Task<Comment?> UpdateCommentAsync(Comment comment)
        {
            try
            {
                var commentDto = await _httpClientService.PutAsync<CommentDto>($"{ApiEndpoints.Comments}/{comment.CommentId}", comment);
                return commentDto?.ToComment();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating comment: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteCommentAsync(long commentId)
        {
            try
            {
                await _httpClientService.DeleteAsync($"{ApiEndpoints.Comments}/{commentId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment: {ex.Message}");
                return false;
            }
        }
    }
}