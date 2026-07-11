using System;
using Frank.Domain.Users;
using Frank.TestUtilities.Fixtures;

namespace Frank.TestUtilities.Builders;

public sealed class UserBuilder
    : TestDataBuilderBase<UserBuilder, User>
{
    private string _firstName = NameFixtures.DefaultFirst;
    private string _lastName = NameFixtures.DefaultLast;
    private string _email = EmailFixtures.Random();
    private string? _phone = PhoneNumberFixtures.Valid;
    private string _externalId = $"test|{Guid.NewGuid()}";
    private UserId? _id = null;

    public UserBuilder WithFirstName(string value) => With(b => b._firstName = value);
    public UserBuilder WithLastName(string value) => With(b => b._lastName = value);
    public UserBuilder WithEmail(string value) => With(b => b._email = value);
    public UserBuilder WithPhone(string? value) => With(b => b._phone = value);
    public UserBuilder WithExternalId(string value) => With(b => b._externalId = value);
    public UserBuilder WithId(Guid id) => With(b => b._id = UserId.From(id));

    public override User Build()
    {
        var first = FirstName.From(_firstName);
        var last = LastName.From(_lastName);
        var email = Email.From(_email);
        var externalId = ExternalId.From(_externalId);
        var phone = _phone is not null ? PhoneNumber.From(_phone) : null;

        var user = User.Create(
            first,
            last,
            email,
            externalId,
            phone
        );

        // Override ID if the test requested a deterministic one
        if (_id is not null)
        {
            typeof(User)
                .GetProperty(nameof(User.Id))!
                .SetValue(user, _id);
        }

        return user;
    }

    /// <summary>
    /// Builds an external-auth user using the domain factory.
    /// </summary>
    public User BuildExternalUser(
        string externalId,
        string firstName = "External",
        string lastName = "User",
        string email = "external@example.com")
    {
        return User.Create(
            firstName: FirstName.From(firstName),
            lastName: LastName.From(lastName),
            email: Email.From(email),
            externalId: ExternalId.From(externalId));
    }

    /// <summary>
    /// Builds an anonymous object representing the API request payload.
    /// </summary>
    public object BuildApiRequest()
    {
        return new
        {
            FirstName = _firstName,
            LastName = _lastName,
            Email = _email,
            Phone = _phone
        };
    }
}
