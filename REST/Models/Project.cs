using Supabase.Core;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace REST.Models
{
    [Table("projects")]
    public partial class Project : BaseModel
    {
        [PrimaryKey("project_id")]
        public long ProjectId { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("status_id")]
        public long StatusId { get; set; }

        [Reference(typeof(Status))]
        public Status Status { get; set; }

        public List<Task> Tasks { get; set; } = new();
    }
}
