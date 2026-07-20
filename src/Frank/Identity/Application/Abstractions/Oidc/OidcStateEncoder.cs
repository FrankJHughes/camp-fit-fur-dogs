using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Frank.Core.Application.Abstractions.Sessions.Oidc;

public class OidcStateEncoder
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static bool TryEncodeValue<T>(T decoded, out string? encoded)
    {
        try
        {
            var json = JsonSerializer.Serialize(decoded, Options);
            var bytes = Encoding.UTF8.GetBytes(json);
            encoded = WebEncoders.Base64UrlEncode(bytes);
            return true;
        }
        catch
        {
            encoded = default;
            return false;
        }
    }

    public static bool TryDecodeValue<T>(string encoded, out T? decoded)
    {
        try
        {
            var bytes = WebEncoders.Base64UrlDecode(encoded);
            var json = Encoding.UTF8.GetString(bytes);
            decoded = JsonSerializer.Deserialize<T>(json, Options);
            return true;
        }
        catch
        {
            decoded = default;
            return false;
        }
    }
}
