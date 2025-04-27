using Supabase.Core;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace REST.Models
{
    [Table("Task")]
    public partial class Task : BaseModel
    {
        [PrimaryKey("task_id")]
        public long TaskId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("project_id")]
        public long ProjectId { get; set; }

        [Reference(typeof(Project))]
        public Project? Project { get; set; }

        [Column("status_id")]
        public long? StatusId { get; set; }

        [Reference(typeof(Status))]
        public Status? Status { get; set; }

        public List<Comment> Comments { get; set; } = new();
    }
}
