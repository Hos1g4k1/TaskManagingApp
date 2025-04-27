namespace REST.Models.DTOs
{
    public class TaskDto
    {
        public long TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Project relationship
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        
        // Status relationship
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        
        // Comments count for lightweight display
        public int CommentCount { get; set; }

        // Static method to convert from entity to DTO
        public static TaskDto FromTask(Task task)
        {
            return new TaskDto
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId,
                ProjectName = task.Project?.Name,
                StatusId = task.StatusId,
                StatusName = task.Status?.Name,
                CommentCount = task.Comments?.Count ?? 0
            };
        }

        // Static method to convert a list of entities to DTOs
        public static List<TaskDto> FromTasks(IEnumerable<Task> tasks)
        {
            return tasks.Select(FromTask).ToList();
        }
    }
} 