using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace Frank.Core.Application.Tests.Fakes;

public sealed class FakeStringResponseQuery : IQuery<string>
{
    public string Payload { get; }

    public FakeStringResponseQuery(string payload)
    {
        Payload = payload;
    }

    public override string ToString() => Payload;
}
