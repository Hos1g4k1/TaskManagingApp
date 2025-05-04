namespace REST.Models.DTOs
{
    public class TaskDependencyDto
    {
        public long DependencyId { get; set; }
        public long TaskId { get; set; }
        public long DependentTaskId { get; set; }
        
        // Include task details for display purposes
        public string TaskTitle { get; set; }
        public string DependentTaskTitle { get; set; }
        
        // Static method to convert from entity to DTO
        public static TaskDependencyDto FromTaskDependency(TaskDependency dependency)
        {
            return new TaskDependencyDto
            {
                DependencyId = dependency.DependencyId,
                TaskId = dependency.TaskId,
                DependentTaskId = dependency.DependentTaskId,
                TaskTitle = dependency.Task?.Title,
                DependentTaskTitle = dependency.DependentTask?.Title
            };
        }

        // Static method to convert a list of entities to DTOs
        public static List<TaskDependencyDto> FromTaskDependencies(IEnumerable<TaskDependency> dependencies)
        {
            return dependencies.Select(FromTaskDependency).ToList();
        }
    }
} 