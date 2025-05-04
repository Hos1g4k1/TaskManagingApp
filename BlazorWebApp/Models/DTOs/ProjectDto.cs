namespace BlazorWebApp.Models.DTOs
{
    public class ProjectDto
    {
        public long ProjectId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? StatusId { get; set; }
        public string? StatusName { get; set; }

        // Helper method to convert DTO to model
        public Project ToProject()
        {
            return new Project
            {
                ProjectId = ProjectId,
                Name = Name,
                Description = Description,
                StartDate = StartDate,
                EndDate = EndDate,
                StatusId = StatusId,
                StatusName = StatusName
            };
        }
    }
}