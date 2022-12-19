namespace CourseLibrary.API.Hateoas;

public class LinkResourceBase
{
    public LinkResourceBase()
    {

    }

    public IEnumerable<Link> Links { get; set; } = new List<Link>();
}
