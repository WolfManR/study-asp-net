namespace Catalog.Books;

public record StoreBooksRequest(Book Book, Author Author, int Amount)
{
    public BookInfo Info() =>
        new BookInfo()
        {
            Title = Book.Title,
            Description = Book.Description,
            Author = Author.FullName,
            Amount = Amount
        };
}