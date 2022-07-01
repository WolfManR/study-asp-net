namespace TemplatesReporter.AuthenticationRules;

public sealed class JwtSettings
{
    public const string AuthPolicy = "AuthPolicy";
    public string SecureCode { get; init; }
    public string ValidAudience { get; init; }
    public string ValidIssuer { get; init; }
}