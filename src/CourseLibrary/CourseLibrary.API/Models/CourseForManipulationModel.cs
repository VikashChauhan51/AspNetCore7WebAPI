using CourseLibrary.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models;

[CourseTitleMustBeDifferentFromDescription]
public record CourseForManipulationModel
{
    [Required(ErrorMessage = "You should fill out a title.")]
    [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 characters.")]
    public string Title { get; init; } = default!;

    [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
    public virtual string Description { get; init; } = string.Empty;
}
