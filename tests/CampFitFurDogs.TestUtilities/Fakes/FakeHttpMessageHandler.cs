namespace CampFitFurDogs.TestUtilities.Fakes;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, HttpResponseMessage> _responses;

    public FakeHttpMessageHandler(Dictionary<string, HttpResponseMessage> responses)
    {
        _responses = responses;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_responses.TryGetValue(request.RequestUri!.ToString(), out var response))
            return Task.FromResult(response);

        throw new InvalidOperationException($"No fake response configured for {request.RequestUri}");
    }
}
