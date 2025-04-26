using Supabase.Core;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace REST.Models
{
    [Table("Comment")]
    public partial class Comment : BaseModel
    {
        [PrimaryKey("comment_id")]
        public long CommentId { get; set; }

        [Column("task_id")]
        public long TaskId { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
