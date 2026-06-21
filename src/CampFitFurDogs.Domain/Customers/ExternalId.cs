using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class ExternalId : ValueObject
{
    public string Value { get; }

    private ExternalId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidExternalAuthProviderIdException("External auth provider ID cannot be empty.");

        value = value.Trim();

        // Domain invariant: must be in the form "provider|identifier"
        // Example: "auth0|abc123"
        var parts = value.Split('|', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            throw new InvalidExternalAuthProviderIdException(
                $"External auth provider ID must be in the format 'provider|id'. Received: '{value}'.");

        if (string.IsNullOrWhiteSpace(parts[0]))
            throw new InvalidExternalAuthProviderIdException("External auth provider name cannot be empty.");

        if (string.IsNullOrWhiteSpace(parts[1]))
            throw new InvalidExternalAuthProviderIdException("External auth provider user ID cannot be empty.");

        Value = value;
    }

    public static ExternalId From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
