using System.ComponentModel.DataAnnotations;

namespace BlazorWebApp.Models
{
    public class TaskDependency
    {
        public long DependencyId { get; set; }

        [Required(ErrorMessage = "Task ID is required")]
        public long TaskId { get; set; }

        [Required(ErrorMessage = "Dependent Task ID is required")]
        public long DependentTaskId { get; set; }

        // Navigation properties for display
        public string? TaskTitle { get; set; }
        public string? DependentTaskTitle { get; set; }

        // Additional info for UI display
        public string? TaskStatusName { get; set; }
        public long? TaskStatusId { get; set; }
    }
}