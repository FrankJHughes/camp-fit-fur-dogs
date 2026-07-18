using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Oidc;

public interface ICallbackOidcContextBuilder : IImmutableContextBuilder<
    CallbackOidcContextBuilderRequest,
    CallbackOidcContext,
    CallbackOidcContextBuilderResult>
{ }
