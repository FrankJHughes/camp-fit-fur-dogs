using System;
using System.Collections.Generic;
using Frank.Abstractions.Observations;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeErrorBoundaryObserver : IErrorBoundaryObserver
{
    public List<Exception> Errors { get; } = [];

    public void OnError(Exception exception, IRequestObservationContext context)
    {
        Errors.Add(exception);
    }
}
