using AutoMapper;
 

namespace CourseLibrary.API.Profiles;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<Author, AuthorModel>()
           .ForMember(dest => dest.Name, opt =>
               opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
           .ForMember(dest => dest.Age, opt =>
               opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));


        CreateMap<AuthorForCreationModel, Author>();

        CreateMap<Author, AuthorFullModel>();

        CreateMap<AuthorForCreationWithDateOfDeathModel, Author>();

    }
}
