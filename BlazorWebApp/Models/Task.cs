using System.ComponentModel.DataAnnotations;

namespace BlazorWebApp.Models
{
    public class TaskItem
    {
        public long TaskId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Project is required")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a project")]
        public long ProjectId { get; set; }

        public string? ProjectName { get; set; }

        public long? StatusId { get; set; }

        public string? StatusName { get; set; }

        public int CommentCount { get; set; }
    }
}