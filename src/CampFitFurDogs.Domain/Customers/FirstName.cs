using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SharedKernel.Domain;

namespace CampFitFurDogs.Domain.Customers;

public sealed class FirstName : ValueObject
{
    private static readonly Regex ValidChars = new(
        @"^[A-Za-z' -]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private FirstName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidFirstNameException("First name cannot be empty.");

        value = value.Trim().Normalize(NormalizationForm.FormC);

        if (value.Length is < 1 or > 100)
            throw new InvalidFirstNameException("First name must be between 1 and 100 characters.");

        if (!ValidChars.IsMatch(value))
            throw new InvalidFirstNameException("First name contains invalid characters.");

        Value = value;
    }

    public static FirstName From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

// public sealed class InvalidFirstNameException : DomainException
// {
//     public InvalidFirstNameException(string message) : base(message) { }
// }
