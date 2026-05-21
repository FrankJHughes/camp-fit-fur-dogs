using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class CustomerBuilder 
    : TestDataBuilderBase<CustomerBuilder, Customer>
{
    private string _firstName = NameFixtures.DefaultFirst;
    private string _lastName = NameFixtures.DefaultLast;
    private string _email = $"test-{Guid.NewGuid()}@example.com";
    private string _phone = PhoneNumberFixtures.Valid;
    private string _password = PasswordFixtures.Plain;

    public CustomerBuilder WithFirstName(string value) => With(b => b._firstName = value);
    public CustomerBuilder WithLastName(string value) => With(b => b._lastName = value);
    public CustomerBuilder WithEmail(string value) => With(b => b._email = value);
    public CustomerBuilder WithPhone(string value) => With(b => b._phone = value);
    public CustomerBuilder WithPassword(string value) => With(b => b._password = value);

    public override Customer Build()
    {
        return Customer.Create(
            FirstName.From(_firstName),
            LastName.From(_lastName),
            Email.From(_email),
            PhoneNumber.From(_phone),
            PasswordHash.Create(_password));
    }

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
