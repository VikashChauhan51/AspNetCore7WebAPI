﻿

using CourseLibrary.API.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Extensions;

public static class RepositoryExtension
{
    public static IServiceCollection CongigureRepositories(this IServiceCollection services, IConfiguration Configuration)
    {
        services.AddScoped<ICourseLibraryService,
           CourseLibraryService>();

        services.AddScoped<IAuthorRepository,
         AuthorRepository>();
        services.AddScoped<ICourseRepository,
         CourseRepository>();

        services.AddDbContext<CourseLibraryContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("CourseLibraryDatabase"));
    });
        services.AddScoped<IValidator<AuthorForCreationModel>, AuthorValidator>();
        services.AddScoped<IValidator<CourseForCreationModel>, CourseValidator>();
        return services;
    }
}
