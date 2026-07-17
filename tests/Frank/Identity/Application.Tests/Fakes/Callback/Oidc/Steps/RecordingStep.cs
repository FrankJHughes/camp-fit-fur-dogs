using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Tests.Fakes.Callback.Oidc.Steps;

public sealed class RecordingStep : IImmutableContextBuildStep<OidcCallbackContext>
{
    private readonly string _id;
    private readonly List<string> _recorder;

    public RecordingStep(string id, List<string> recorder)
    {
        _id = id;
        _recorder = recorder;
    }

    public bool CanExecute(OidcCallbackContext ctx) => true;

    public Task<OidcCallbackContext> ExecuteAsync(OidcCallbackContext ctx, CancellationToken ct)
    {
        _recorder.Add(_id);
        throw new InvalidOperationException("Stop pipeline");
    }

    public IImmutableContextBuildStepMetadata Metadata => new ImmutableContextBuildStepMetadata("Record", "Record");
}
