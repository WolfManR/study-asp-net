using Catalog.Books;
using Catalog.FullTextSearch;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers;

[Route("books")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;
    private readonly ElasticService _elasticService;

    public BooksController(IBooksRepository booksRepository, ElasticService elasticService)
    {
        _booksRepository = booksRepository;
        _elasticService = elasticService;
    }

    [HttpPost("store")]
    public async Task<IActionResult> Store([FromBody] StoreBooksRequest request)
    {
        await _booksRepository.Store(request.Book, request.Author, request.Amount);
        _elasticService.IndexBook(request.Info());
        return Ok();
    }

    [HttpPost("count/{bookTitle}")]
    public async Task<IActionResult> CountStored([FromQuery] string bookTitle)
    {
        var count = await _booksRepository.CountStoredBooks(bookTitle);
        return Ok(count);
    }

    [HttpGet]
    public IAsyncEnumerable<BookInfo> ListStored()
    {
        return _booksRepository.ListBooks();
    }

    [HttpGet("search/{bookTitle}")]
    public async Task<IActionResult> Search([FromQuery] string bookTitle)
    {
        var result = await _elasticService.Search(bookTitle);
        return Ok(result);
    }
}
