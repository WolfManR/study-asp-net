namespace Catalog.Books;

public interface IBooksRepository
{
    Task<long> CountStoredBooks(string bookTitle);
    IAsyncEnumerable<BookInfo> ListBooks();
    Task<bool> Store(Book book, Author author, long count);
}