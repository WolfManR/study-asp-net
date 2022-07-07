using Neo4j.Driver;

namespace Neo4j.Core;

public class Neo4jContext : IAsyncDisposable, IDisposable
{
    private bool _isDisposed;
    private bool _isDisposing;
    private readonly IDriver _driver;

    public Neo4jContext(Neo4jOptions options)
    {
        _driver = GraphDatabase.Driver(options.Uri, options.GenerateAuthTokens());
    }

    /// <summary>
    /// Obtain a session with the default <see cref="T:Neo4j.Driver.SessionConfig" />.
    /// </summary>
    /// <returns>An <see cref="T:Neo4j.Driver.IAsyncSession" /> that could be used to execute queries.</returns>
    public IAsyncSession AsyncSession() => _driver.AsyncSession();

    /// <summary>
    /// Obtain a session with the customized <see cref="T:Neo4j.Driver.SessionConfig" />.
    /// </summary>
    /// <param name="action">An action, provided with a <see cref="T:Neo4j.Driver.SessionConfigBuilder" /> instance, that should populate
    /// the provided instance with desired <see cref="T:Neo4j.Driver.SessionConfig" />.</param>
    /// <returns>An <see cref="T:Neo4j.Driver.IAsyncSession" /> that could be used to execute queries.</returns>
    public IAsyncSession AsyncSession(Action<SessionConfigBuilder> action) => _driver.AsyncSession();

    /// <summary>
    /// Asynchronously releases all resources (connection pools, connections, etc) associated with this IDriver instance.
    /// </summary>
    /// <returns>The close task.</returns>
    public Task CloseAsync() => _driver.CloseAsync();

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed || _isDisposing) return;
        _isDisposing = true;
        await CloseAsync();
        Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            _driver.Dispose();
        }

        _isDisposed = true;
    }

    ~Neo4jContext() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}