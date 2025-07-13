namespace BlazorWebApp.Models.DTOs
{
    public class TaskDependencyDto
    {
        public long DependencyId { get; set; }
        public long TaskId { get; set; }
        public long DependentTaskId { get; set; }
        public string? TaskTitle { get; set; }
        public string? DependentTaskTitle { get; set; }

        public TaskDependency ToTaskDependency()
        {
            return new TaskDependency
            {
                DependencyId = DependencyId,
                TaskId = TaskId,
                DependentTaskId = DependentTaskId,
                TaskTitle = TaskTitle,
                DependentTaskTitle = DependentTaskTitle
            };
        }

        public static TaskDependencyDto FromTaskDependency(TaskDependency dependency)
        {
            return new TaskDependencyDto
            {
                DependencyId = dependency.DependencyId,
                TaskId = dependency.TaskId,
                DependentTaskId = dependency.DependentTaskId,
                TaskTitle = dependency.TaskTitle,
                DependentTaskTitle = dependency.DependentTaskTitle
            };
        }
    }
}