using Catalog.Books;
using Refit;

namespace Catalog.Site.Services;

public interface IBooksCatalogClient
{
    [Post("/books/store")]
    Task Store([Body] StoreBooksRequest request);

    [Post("books/count/{bookTitle}")]
    Task<int> CountStored([Query] string bookTitle);

    [Get("/books")]
    // TODO: change to async enumerable when it will be released
    Task<IEnumerable<BookResponse>> ListStored();
}