using Frank.Abstractions;

namespace Frank.Tests.Fakes;

public sealed class FakeStringResponseQuery : IQuery<string>
{
    public string Payload { get; }

    public FakeStringResponseQuery(string payload)
    {
        Payload = payload;
    }

    public override string ToString() => Payload;
}
