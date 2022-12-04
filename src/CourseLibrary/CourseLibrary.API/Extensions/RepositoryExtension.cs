

using CourseLibrary.API.DbContexts;
using Microsoft.AspNetCore.Identity;

namespace CourseLibrary.API.Extensions;

public static class RepositoryExtension
{
    public static IServiceCollection CongigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICourseLibraryService,
           CourseLibraryService>();

        services.AddScoped<IAuthorRepository,
         AuthorRepository>();
        services.AddScoped<ICourseRepository,
         CourseRepository>();

        services.AddDbContext<CourseLibraryContext>(options =>
        {
        });
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<IValidator<AuthorModel>, AuthorValidator>();
        services.AddScoped<IValidator<CourseModel>, CourseValidator>();
        return services;
    }
}
