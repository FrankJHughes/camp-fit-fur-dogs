using System.Net.Http.Json;

namespace Frank.TestUtilities.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T> GetJsonAsync<T>(this HttpClient client, string url)
        => await client.GetFromJsonAsync<T>(url)
           ?? throw new InvalidOperationException($"GET {url} returned null JSON");
}
