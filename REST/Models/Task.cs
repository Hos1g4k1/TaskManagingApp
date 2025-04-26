using Supabase.Core;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace REST.Models
{
    [Table("tasks")]
    public partial class Task : BaseModel
    {
        [PrimaryKey("task_id")]
        public long TaskId { get; set; }

        [Column("project_id")]
        public long ProjectId { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("status_id")]
        public long StatusId { get; set; }

        [Reference(typeof(Status))]
        public Status Status { get; set; }

        [Reference(typeof(Project))]
        public Project Project { get; set; }

        public List<TaskDependency> Dependencies { get; set; } = new();

        public List<Comment> Comments { get; set; } = new(); 
    }
}
