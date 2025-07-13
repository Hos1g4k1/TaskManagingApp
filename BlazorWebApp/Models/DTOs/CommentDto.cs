namespace BlazorWebApp.Models.DTOs
{
    public class CommentDto
    {
        public long CommentId { get; set; }
        public long TaskId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Comment ToComment()
        {
            return new Comment
            {
                CommentId = CommentId,
                TaskId = TaskId,
                Content = Content,
                CreatedAt = CreatedAt
            };
        }

        public static CommentDto FromComment(Comment comment)
        {
            return new CommentDto
            {
                CommentId = comment.CommentId,
                TaskId = comment.TaskId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }
    }
}