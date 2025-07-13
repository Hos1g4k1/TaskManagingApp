using System.ComponentModel.DataAnnotations;

namespace BlazorWebApp.Models
{
    public class Comment
    {
        public long CommentId { get; set; }

        [Required(ErrorMessage = "Task is required")]
        public long TaskId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(2000, ErrorMessage = "Content cannot exceed 2000 characters")]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}