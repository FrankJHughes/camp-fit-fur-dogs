namespace Frank.Testing.Contexts;

public abstract record MutatedWebApplicationClientContext<TSelf>
    where TSelf : MutatedWebApplicationClientContext<TSelf>
{
    public string? AuthenticatedUserSub { get; init; }
    public string? SignInScheme { get; init; }   // API-specific, passed in by consumer
    public Dictionary<string, string> DefaultHeaders { get; init; } = new();
}
