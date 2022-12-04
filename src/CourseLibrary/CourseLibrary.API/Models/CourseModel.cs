namespace CourseLibrary.API.Models
{
    public record CourseModel
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = default!;

        public string? Description { get; init; }
    }
}
