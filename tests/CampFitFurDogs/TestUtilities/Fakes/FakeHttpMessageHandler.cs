namespace CampFitFurDogs.TestUtilities.Fakes;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, HttpResponseMessage> _responses;

    public FakeHttpMessageHandler()
        : this([])
    {
    }

    public FakeHttpMessageHandler(Dictionary<string, HttpResponseMessage> responses)
    {
        _responses = responses;
    }

    public void Add(string url, HttpResponseMessage response)
    {
        _responses[url] = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var url = request.RequestUri!.ToString();

        if (_responses.TryGetValue(url, out var response))
            return Task.FromResult(response);

        throw new InvalidOperationException(
            $"No fake response configured for {url}");
    }
}
