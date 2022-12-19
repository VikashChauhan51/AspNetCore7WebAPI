namespace CourseLibrary.API.Hateoas;

public class Link
{
    public string Href { get; init; } = string.Empty;
    public string Rel { get; init; } = string.Empty;
    public string Method { get; init; } = string.Empty;

    public Link()
    {

    }

    public Link(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}
