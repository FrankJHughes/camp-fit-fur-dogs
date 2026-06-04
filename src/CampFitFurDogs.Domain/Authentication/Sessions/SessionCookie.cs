using Frank.Domain;

namespace CampFitFurDogs.Domain.Authentication.Sessions;

public sealed class SessionCookie : ValueObject
{
    public string Name { get; }
    public string Value { get; }

    private SessionCookie(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public static SessionCookie FromPlaintextToken(string token)
        => new("cfd.session", token);

    public override string ToString() => $"{Name}={Value}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Value;
    }
}
