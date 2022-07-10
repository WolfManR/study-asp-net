using Catalog.Books;

using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers;

[Route("books")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }

    [HttpPost("store")]
    public async Task<IActionResult> Store([FromBody] StoreBooksRequest request)
    {
        await _booksRepository.Store(request.Book, request.Author, request.Amount);
        return Ok();
    }

    [HttpPost("count/{bookTitle}")]
    public async Task<IActionResult> CountStored([FromQuery] string bookTitle)
    {
        var count = await _booksRepository.CountStoredBooks(bookTitle);
        return Ok(count);
    }

    [HttpGet]
    public async IAsyncEnumerable<BookResponse> ListStored()
    {
        BookResponse cache = new();
        await foreach (var book in _booksRepository.ListBooks())
        {
            cache.Title = book.Title;
            cache.Description = book.Description;
            cache.Author = $"{book.AuthorFirstName} {book.AuthorLastName}";
            cache.Amount = book.Amount;
            yield return cache;
        }
    }
}
