using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Frank.TestUtilities.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T> GetJsonAsync<T>(this HttpClient client, string url)
        => await client.GetFromJsonAsync<T>(url)
           ?? throw new InvalidOperationException($"GET {url} returned null JSON");
}
