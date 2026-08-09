using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Frank.Core.Application.Abstractions.Sessions.Oidc;

/// <summary>
/// Provides utility methods for encoding and decoding OIDC state values using
/// JSON serialization and Base64URL encoding.
/// <para>
/// This encoder is typically used to safely round‑trip structured state objects
/// through OIDC authorization requests, where values must be URL‑safe and
/// resilient to tampering or formatting issues.
/// </para>
/// <para>
/// The encoding format is:
/// <c>JSON → UTF‑8 bytes → Base64URL</c>.
/// The decoding format reverses this process.
/// </para>
/// </summary>
/// <remarks>
/// The methods in this class follow a <c>TryXxx</c> pattern, returning
/// <c>false</c> when encoding or decoding fails rather than throwing exceptions.
/// This makes the encoder safe to use in pipeline and middleware scenarios where
/// malformed state values must be handled gracefully.
/// </remarks>
public class OidcStateEncoder
{
    /// <summary>
    /// Shared JSON serializer options used for encoding and decoding state
    /// objects.
    /// Uses camel‑case naming and no indentation to minimize payload size.
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Attempts to encode a value into a Base64URL‑encoded JSON string.
    /// <para>
    /// The value is serialized using <see cref="JsonSerializer"/>, converted to
    /// UTF‑8 bytes, and then encoded using
    /// <see cref="WebEncoders.Base64UrlEncode(byte[])"/>.
    /// </para>
    /// <para>
    /// If encoding fails (e.g., due to serialization errors), the method returns
    /// <c>false</c> and sets <paramref name="encoded"/> to <c>null</c>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to encode.
    /// Must be serializable by <see cref="JsonSerializer"/>.
    /// </typeparam>
    /// <param name="decoded">The value to encode.</param>
    /// <param name="encoded">
    /// When the method returns <c>true</c>, contains the Base64URL‑encoded JSON
    /// representation of <paramref name="decoded"/>.
    /// Otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if encoding succeeds; otherwise <c>false</c>.
    /// </returns>
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

    /// <summary>
    /// Attempts to decode a Base64URL‑encoded JSON string into a typed value.
    /// <para>
    /// The method reverses the encoding process:
    /// <c>Base64URL → UTF‑8 → JSON → object</c>.
    /// </para>
    /// <para>
    /// If decoding fails (e.g., invalid Base64URL input, malformed JSON, or
    /// deserialization errors), the method returns <c>false</c> and sets
    /// <paramref name="decoded"/> to <c>null</c>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// The type into which the decoded JSON should be deserialized.
    /// </typeparam>
    /// <param name="encoded">
    /// The Base64URL‑encoded JSON string to decode.
    /// </param>
    /// <param name="decoded">
    /// When the method returns <c>true</c>, contains the deserialized value.
    /// Otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if decoding succeeds; otherwise <c>false</c>.
    /// </returns>
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
