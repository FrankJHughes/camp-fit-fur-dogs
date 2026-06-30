namespace Frank.Abstractions.ImmutableContext;

public sealed class ImmutableContextBuildStepMetadata : IImmutableContextBuildStepMetadata
{
    public ImmutableContextBuildStepMetadata(string id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public string Id { get; }
    public string DisplayName { get; }
}
