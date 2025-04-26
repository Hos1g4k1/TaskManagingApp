using REST.Models;
using REST.Models.DTOs;

namespace REST.Repositories
{
    public interface ICommentRepository
    {
        // Entity methods
        Task<List<Comment>> GetAllCommentsAsync();
        Task<Comment> GetCommentByIdAsync(long commentId);
        Task<List<Comment>> GetCommentsByTaskIdAsync(long taskId);
        Task<Comment> AddCommentAsync(Comment comment);
        Task<Comment> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(long commentId);

        // DTO methods
        Task<List<CommentDto>> GetAllCommentDtosAsync();
        Task<CommentDto> GetCommentDtoByIdAsync(long commentId);
        Task<List<CommentDto>> GetCommentDtosByTaskIdAsync(long taskId);
    }
}