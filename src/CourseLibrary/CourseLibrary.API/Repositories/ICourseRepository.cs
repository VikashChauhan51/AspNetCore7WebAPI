using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Repositories
{
    public interface ICourseRepository : IRepository
    {
        void AddCourse(Guid authorId, Course course);
        void DeleteCourse(Course course);
        Task<Course?> GetCourseAsync(Guid courseId);
        Task<IEnumerable<Course>> GetCoursesAsync(Guid authorId);
        void UpdateCourse(Course course);
        Task<IEnumerable<Course>> GetCoursesAsync();
    }
}