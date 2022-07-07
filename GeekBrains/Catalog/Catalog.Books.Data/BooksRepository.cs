using Neo4j.Core;
using Neo4j.Driver;

namespace Catalog.Books.Data;

public class BooksRepository : IBooksRepository
{
    private readonly Neo4jContext _context;

    public BooksRepository(Neo4jContext context)
    {
        _context = context;
    }

    public async Task<long> CountStoredBooks(string bookTitle)
    {
        await using var session = _context.AsyncSession();

        var result = await session.ReadTransactionAsync(tx => CountOfStoredBooks(tx, bookTitle));

        return result;
    }

    public async IAsyncEnumerable<BookInfo> ListBooks()
    {
        await using var session = _context.AsyncSession();

        var result = await session.ReadTransactionAsync(GetAllBooks);

        foreach (var book in result)
        {
            yield return book;
        }
    }

    public async Task<bool> Store(Book book, Author author, long amount)
    {
        await using var session = _context.AsyncSession();

        try
        {
            await session.WriteTransactionAsync(tx => AddBook(tx, book, author));
            var (isStored, previousCount) = await session.ReadTransactionAsync(tx => CheckThatBookStored(tx, book));

            if (isStored)
            {
                await session.WriteTransactionAsync(tx => UpdateCountOfStoredBooks(tx, book, previousCount + amount));
                return true;
            }

            await session.WriteTransactionAsync(tx => AddBookToStorage(tx, book, amount));

            return true;
        }
        catch (Neo4jException ex)
        {
            return false;
        }
    }

    // create book with single author
    //merge (b:Book { title: 'Some story' , description: 'some described here', pages: 3 })
    //merge (a:Person:Author { firstname: 'Somebody', lastname: 'La Cruse' })
    //merge (a) <- [w:WRITE] - (b)

    // check that book stored
    //match(a:Book { title: 'Other story' })
    //with a
    //merge(s:Storage)
    //with a, s
    //optional match(s) - [st: Stored] - (a)
    //return st,a

    // add book to storage
    //match(a:Book { title: 'Other story' })
    //match(s:Storage)
    //create(s) <- [st: Stored {count:84}] - (a)

    // update Count of Stored books
    //match(s:Storage) <- [st: Stored] - (a:Book { title: 'Other story' })
    //set st += {count: 46}

    // get books
    //match (b:Book) return b

    private async Task<IResultSummary> AddBook(IAsyncTransaction transaction, Book book, Author author)
    {
        const string query = @"
merge (b:Book { title: $title , description: $description, pages: $pages })
merge (a:Person:Author { firstName: $firstName, lastName: $lastName })
merge (a) - [w:WRITE] -> (b)
";

        var cursor = await transaction.RunAsync(query, new
        {
            title = book.Title,
            description = book.Description,
            pages = book.Pages,

            firstName = author.FirstName,
            lastName = author.LastName
        });

        return await cursor.ConsumeAsync();
    }
    private async Task<(bool, long)> CheckThatBookStored(IAsyncTransaction transaction, Book book)
    {
        const string query = @"
match (a:Book { title: $title , description: $description, pages: $pages })
with a
merge (s:Storage)
with a, s
optional match(s) - [st: Stored] - (a)
return st.count as count
";

        var cursor = await transaction.RunAsync(query, new
        {
            title = book.Title,
            description = book.Description,
            pages = book.Pages
        });

        if (!await cursor.FetchAsync()) return (false, default);

        var storedBooksCount = cursor.Current["count"];
        return storedBooksCount is null
            ? (false, default)
            : (true, storedBooksCount.As<long>());
    }
    private async Task<IResultSummary> AddBookToStorage(IAsyncTransaction transaction, Book book, long amount)
    {
        const string query = @"
match (a:Book { title: $title , description: $description, pages: $pages })
match(s:Storage)
create(s) <- [st: Stored {count: $amount}] - (a)
";

        var cursor = await transaction.RunAsync(query, new
        {
            title = book.Title,
            description = book.Description,
            pages = book.Pages,
            amount
        });

        return await cursor.ConsumeAsync();
    }
    private async Task<IResultSummary> UpdateCountOfStoredBooks(IAsyncTransaction transaction, Book book, long amount)
    {
        const string query = @"
match(s:Storage) <- [st: Stored] - (a:Book { title: $title , description: $description, pages: $pages })
set st += {count: $amount}
";

        var cursor = await transaction.RunAsync(query, new
        {
            title = book.Title,
            description = book.Description,
            pages = book.Pages,
            amount
        });

        return await cursor.ConsumeAsync();
    }

    private async Task<IReadOnlyCollection<BookInfo>> GetAllBooks(IAsyncTransaction transaction)
    {
        const string query = @"
match (b:Book) <- - (a:Author)
return b.title as title, b.description as description, a.firstName as authorFirstName, a.lastName as authorLastName";

        var cursor = await transaction.RunAsync(query);

        return await cursor.ToListAsync(record => new BookInfo()
        {
            Title = record["title"].As<string>(),
            Description = record["description"].As<string>(),
            AuthorFirstName = record["authorFirstName"].As<string>(),
            AuthorLastName = record["authorLastName"].As<string>()
        });
    }
    private async Task<long> CountOfStoredBooks(IAsyncTransaction transaction, string bookTitle)
    {
        const string query = @"
match (a:Book { title: $title}) - [st:Stored] - (s:Storage)
return st.count as count
";

        var cursor = await transaction.RunAsync(query, new { title = bookTitle });

        if (!await cursor.FetchAsync()) return default;

        var storedBooksCount = cursor.Current["count"];
        return storedBooksCount?.As<long>() ?? default;
    }
}