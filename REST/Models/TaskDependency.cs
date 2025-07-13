using Supabase.Core;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace REST.Models
{
    [Table("TaskDependency")]
    public partial class TaskDependency : BaseModel
    {
        [PrimaryKey("dependency_id")]
        public long DependencyId { get; set; }

        [Column("task_id")]
        public long TaskId { get; set; }

        [Column("dependent_task_id")]
        public long DependentTaskId { get; set; }

        // Navigation properties - loaded manually to avoid Supabase ORM confusion
        public Task? Task { get; set; }
        public Task? DependentTask { get; set; }
    }
}

// TaskManagement123!
