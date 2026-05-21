using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class LastName : ValueObject
{
    private static readonly Regex ValidChars = new(
        @"^[A-Za-z' -]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidLastNameException("Last name cannot be empty.");

        value = value.Trim().Normalize(NormalizationForm.FormC);

        if (value.Length is < 1 or > 100)
            throw new InvalidLastNameException("Last name must be between 1 and 100 characters.");

        if (!ValidChars.IsMatch(value))
            throw new InvalidLastNameException("Last name contains invalid characters.");

        Value = value;
    }

    public static LastName From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

// public sealed class InvalidLastNameException : DomainException
// {
//     public InvalidLastNameException(string message) : base(message) { }
// }
