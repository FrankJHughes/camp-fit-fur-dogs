using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Save;

public interface ICallbackSaveContextBuilder : IImmutableContextBuilder<
    CallbackSaveContextBuilderRequest,
    CallbackSaveContext,
    CallbackSaveContextBuilderResult>
{ }
