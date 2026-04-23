namespace SharedKernel.Tests.Fakes;

public sealed class FakeStringResponseCommand : ICommand<string>
{
    public string Payload { get; }

    public FakeStringResponseCommand(string payload)
    {
        Payload = payload;
    }

    public override string ToString() => Payload;
}
