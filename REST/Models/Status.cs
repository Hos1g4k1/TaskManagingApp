using Supabase.Core;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace REST.Models
{
    [Table("Status")]
    public partial class Status : BaseModel
    {
        [PrimaryKey("status_id")]
        public long StatusId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }
    }
}
