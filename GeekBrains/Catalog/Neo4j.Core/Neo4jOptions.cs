using Neo4j.Driver;

namespace Neo4j.Core;

public class Neo4jOptions
{
    public string Uri { get; init; }
    public string Login { get; init; }
    public string Password { get; init; }

    public IAuthToken GenerateAuthTokens()
    {
        return AuthTokens.Basic(Login, Password);
    }
}