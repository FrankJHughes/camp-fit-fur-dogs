using Frank.Abstractions.ImmutableContextBuilder;
using Frank.Authentication.Callback.Oidc;

namespace Frank.Tests.Fakes.Authentication.Callback.Oidc.Steps;

public sealed class RecordingStep : IImmutableContextBuildStep<OidcAuthCallbackContext>
{
    private readonly string _id;
    private readonly List<string> _recorder;

    public RecordingStep(string id, List<string> recorder)
    {
        _id = id;
        _recorder = recorder;
    }

    public bool CanExecute(OidcAuthCallbackContext ctx) => true;

    public Task<OidcAuthCallbackContext> ExecuteAsync(OidcAuthCallbackContext ctx, CancellationToken ct)
    {
        _recorder.Add(_id);
        throw new InvalidOperationException("Stop pipeline");
    }

    public IImmutableContextBuildStepMetadata Metadata => new ImmutableContextBuildStepMetadata("Record", "Record");
}
