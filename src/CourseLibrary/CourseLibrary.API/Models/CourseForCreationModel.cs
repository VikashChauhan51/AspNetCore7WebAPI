namespace CourseLibrary.API.Models
{
    public record CourseForCreationModel
    {
        public string Title { get; init; } = default!;
        public string Description { get; init; } = default!;
    }
}
