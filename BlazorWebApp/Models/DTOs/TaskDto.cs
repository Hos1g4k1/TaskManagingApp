namespace BlazorWebApp.Models.DTOs
{
    public class TaskDto
    {
        public long TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public long ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public long? StatusId { get; set; }
        public string? StatusName { get; set; }
        public int CommentCount { get; set; }

        public TaskItem ToTask()
        {
            return new TaskItem
            {
                TaskId = TaskId,
                Title = Title,
                Description = Description,
                DueDate = DueDate,
                CreatedAt = CreatedAt,
                ProjectId = ProjectId,
                ProjectName = ProjectName,
                StatusId = StatusId,
                StatusName = StatusName,
                CommentCount = CommentCount
            };
        }

        public static TaskDto FromTask(TaskItem task)
        {
            return new TaskDto
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId,
                ProjectName = task.ProjectName,
                StatusId = task.StatusId,
                StatusName = task.StatusName,
                CommentCount = task.CommentCount
            };
        }
    }
}