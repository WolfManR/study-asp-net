namespace Catalog.Books;

public record BookResponse()
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public long Amount { get; set; }
}