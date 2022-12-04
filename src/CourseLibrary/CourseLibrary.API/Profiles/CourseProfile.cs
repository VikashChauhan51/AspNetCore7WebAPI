using AutoMapper;

namespace CourseLibrary.API.Profiles; 
public class CourseProfile: Profile
{
    public CourseProfile()
    {
        CreateMap<Entities.Course, Models.CourseModel>();
        CreateMap<Models.CourseForCreationModel, Entities.Course>();
    }
}
