namespace BlazorWebApp.Models
{
    public class Project
    {
        public long ProjectId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? StatusId { get; set; }
        public string? StatusName { get; set; }
    }
}