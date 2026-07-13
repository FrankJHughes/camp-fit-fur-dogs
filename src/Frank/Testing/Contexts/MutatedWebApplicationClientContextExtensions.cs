namespace Frank.Testing.Contexts;

public static class MutatedWebApplicationClientContextExtensions
{
    public static TSelf WithAuthenticatedUser<TSelf>(
        this TSelf ctx,
        string? sub)
        where TSelf : MutatedWebApplicationClientContext<TSelf>
        => ctx with { AuthenticatedUserSub = sub };

    public static TSelf WithHeader<TSelf>(
        this TSelf ctx,
        string key,
        string value)
        where TSelf : MutatedWebApplicationClientContext<TSelf>
    {
        var copy = new Dictionary<string, string>(ctx.DefaultHeaders)
        {
            [key] = value
        };
        return ctx with { DefaultHeaders = copy };
    }

    public static TSelf WithSignInScheme<TSelf>(
        this TSelf ctx,
        string scheme)
        where TSelf : MutatedWebApplicationClientContext<TSelf>
        => ctx with { SignInScheme = scheme };
}
