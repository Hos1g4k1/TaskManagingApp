namespace REST.Models.DTOs
{
    public class CommentDto
    {
        public long CommentId { get; set; }
        public long TaskId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        // Static method to convert from entity to DTO
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

        // Static method to convert a list of entities to DTOs
        public static List<CommentDto> FromComments(IEnumerable<Comment> comments)
        {
            return comments.Select(FromComment).ToList();
        }
    }

    public class TaskBasicInfoDto
    {
        public long TaskId { get; set; }
        public string Title { get; set; }
        public long? StatusId { get; set; }

        public static TaskBasicInfoDto FromTask(Task task)
        {
            return new TaskBasicInfoDto
            {
                TaskId = task.TaskId,
                Title = task.Title,
                StatusId = task.StatusId
            };
        }
    }
}