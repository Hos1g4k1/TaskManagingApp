namespace REST.Models.DTOs
{
    public class StatusDto
    {
        public long StatusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Static method to convert from entity to DTO
        public static StatusDto FromStatus(Status status)
        {
            return new StatusDto
            {
                StatusId = status.StatusId,
                Name = status.Name,
                Description = status.Description
            };
        }

        // Static method to convert a list of entities to DTOs
        public static List<StatusDto> FromStatuses(IEnumerable<Status> statuses)
        {
            return statuses.Select(FromStatus).ToList();
        }
    }
}