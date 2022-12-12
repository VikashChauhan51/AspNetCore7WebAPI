using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models;
public record CourseForUpdateModel: CourseForManipulationModel
{
    [Required(ErrorMessage = "You should fill out a description.")]
    public override string Description
    {
        get => base.Description; init => base.Description = value;
    }
}
