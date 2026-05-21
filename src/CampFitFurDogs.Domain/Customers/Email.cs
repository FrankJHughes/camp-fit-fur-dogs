using System.Text.RegularExpressions;
using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed partial class Email : ValueObject
{
    private static readonly Regex EmailRegex = MyRegex();

    public string Value { get; }

    private Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("Email cannot be empty.");

        value = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(value))
            throw new InvalidEmailException($"Invalid email format: '{value}'.");

        Value = value;
    }

    public static Email From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
    [GeneratedRegex(@"^(?!\.)[A-Za-z0-9._%+-]+@(?!-)([A-Za-z0-9-]+\.)+[A-Za-z]{2,63}$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex MyRegex();
}

// public sealed class InvalidEmailException : DomainException
// {
//     public InvalidEmailException(string message) : base(message) { }
// }
