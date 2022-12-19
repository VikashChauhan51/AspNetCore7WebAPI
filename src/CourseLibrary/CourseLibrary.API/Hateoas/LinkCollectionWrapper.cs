namespace CourseLibrary.API.Hateoas;

public class LinkCollectionWrapper<T> : LinkResourceBase
{
    public IEnumerable<T> Value { get; set; } = new List<T>();

    public LinkCollectionWrapper()
    {

    }

    public LinkCollectionWrapper(List<T> value)
    {
        Value = value;
    }
}

