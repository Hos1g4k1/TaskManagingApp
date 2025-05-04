namespace BlazorWebApp.Models.DTOs
{
    public class StatusDto
    {
        public long StatusId { get; set; }
        public string? Name { get; set; }

        public Status ToStatus()
        {
            return new Status
            {
                StatusId = StatusId,
                Name = Name
            };
        }
    }
}