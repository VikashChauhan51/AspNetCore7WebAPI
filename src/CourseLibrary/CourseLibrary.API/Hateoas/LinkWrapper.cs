namespace CourseLibrary.API.Hateoas;

public class LinkWrapper<T> : LinkResourceBase
{
    public T Value { get; set; } = default!;
    public LinkWrapper()
    {

    }

}
