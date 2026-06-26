using System;
using System.Collections.Generic;
using Frank.Abstractions.Observability;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeErrorBoundaryObserver : IErrorBoundaryObserver
{
    public List<Exception> Errors { get; } = [];

    public void OnError(Exception exception, IRequestObservabilityContext context)
    {
        Errors.Add(exception);
    }
}
