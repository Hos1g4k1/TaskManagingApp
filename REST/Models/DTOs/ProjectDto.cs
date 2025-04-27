namespace REST.Models.DTOs
{
    public class ProjectDto
    {
        public long ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? StatusId { get; set; }
        public string? StatusName { get; set; }

        // Static method to convert from entity to DTO
        public static ProjectDto FromProject(Project project)
        {
            return new ProjectDto
            {
                ProjectId = project.ProjectId,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                StatusId = project.StatusId,
                StatusName = project.Status?.Name
            };
        }

        // Static method to convert a list of entities to DTOs
        public static List<ProjectDto> FromProjects(IEnumerable<Project> projects)
        {
            return projects.Select(FromProject).ToList();
        }
    }
} 