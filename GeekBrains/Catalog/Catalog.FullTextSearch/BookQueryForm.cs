namespace Catalog.FullTextSearch;

public class BookQueryForm
{
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsEmpty => Title is null && FirstName is null && LastName is null;
}