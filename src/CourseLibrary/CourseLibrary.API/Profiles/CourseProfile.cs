using AutoMapper;

namespace CourseLibrary.API.Profiles; 
public class CourseProfile: Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseModel>();
        CreateMap<CourseForCreationModel,Course>();
        CreateMap<Course, CourseForCreationModel>();
    }
}
