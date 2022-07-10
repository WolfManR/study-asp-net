using Catalog.Books;

using Microsoft.Extensions.Options;

using Nest;

namespace Catalog.FullTextSearch;

public class ElasticService
{
    private readonly ElasticClient _client;

    public ElasticService(IOptions<ElasticConfiguration> options)
    {
        var configuration = options.Value;
        _client = new ElasticClient(BuildConnectionSetting(configuration));
    }

    public void IndexBooks(IReadOnlyCollection<BookInfo> books)
    {
        _client.BulkAsync(b => b.IndexMany(books));
    }

    public void IndexBook(BookInfo book)
    {
        _client.IndexDocument(book);
    }

    public async Task<IReadOnlyCollection<BookInfo>> Search(string title = "")
    {
        var result = await _client.SearchAsync<BookInfo>(d => d
            .Query(q => q.Match(m => m.Field(b => b.Title).Query(title)))
            .Size(100)
        );
        return result.Documents;
    }

    private static IConnectionSettingsValues BuildConnectionSetting(ElasticConfiguration configuration)
    {
        Uri uri = new(configuration.Uri);
        return new ConnectionSettings(uri)
            .DefaultMappingFor<BookInfo>(m => m.IndexName(configuration.Index));
    }
}