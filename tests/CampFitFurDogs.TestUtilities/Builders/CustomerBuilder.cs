using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class CustomerBuilder
    : TestDataBuilderBase<CustomerBuilder, Customer>
{
    private string _firstName = NameFixtures.DefaultFirst;
    private string _lastName = NameFixtures.DefaultLast;
    private string _email = EmailFixtures.Random();
    private string? _phone = PhoneNumberFixtures.Valid;
    private string? _password = PasswordFixtures.Plain;
    private string _externalId = $"test|{Guid.NewGuid()}";
    private CustomerId? _id = null;

    public CustomerBuilder WithFirstName(string value) => With(b => b._firstName = value);
    public CustomerBuilder WithLastName(string value) => With(b => b._lastName = value);
    public CustomerBuilder WithEmail(string value) => With(b => b._email = value);
    public CustomerBuilder WithPhone(string? value) => With(b => b._phone = value);
    public CustomerBuilder WithPassword(string? value) => With(b => b._password = value);
    public CustomerBuilder WithExternalId(string value) => With(b => b._externalId = value);
    public CustomerBuilder WithId(Guid id) => With(b => b._id = CustomerId.From(id));

    public override Customer Build()
    {
        var first = FirstName.From(_firstName);
        var last = LastName.From(_lastName);
        var email = Email.From(_email);
        var externalId = ExternalId.From(_externalId);
        var phone = _phone is not null ? PhoneNumber.From(_phone) : null;

        var customer = Customer.Create(
            first,
            last,
            email,
            externalId,
            phone
        );

        // Override ID if the test requested a deterministic one
        if (_id is not null)
        {
            typeof(Customer)
                .GetProperty(nameof(Customer.Id))!
                .SetValue(customer, _id);
        }

        return customer;
    }

    /// <summary>
    /// Builds an external-auth customer using the domain factory.
    /// </summary>
    public Customer BuildExternalUser(
        string externalId,
        string firstName = "External",
        string lastName = "User",
        string email = "external@example.com")
    {
        return Customer.Create(
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
            Phone = _phone,
            Password = _password
        };
    }
}
