using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Customers.Exceptions;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class CustomerTests
{
    private static FirstName ValidFirstName => FirstName.From(NameFixtures.DefaultFirst);
    private static LastName ValidLastName => LastName.From(NameFixtures.DefaultLast);
    private static Email ValidEmail => Email.From(EmailFixtures.Default);
    private static PhoneNumber ValidPhone => PhoneNumber.From(PhoneNumberFixtures.Valid);
    private static PasswordHash ValidHash => PasswordFixtures.Hash();

    // ───────────────────────────────────────────────────────────────
    // 1. LOCAL IDENTITY — Password only
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_LocalIdentity_Succeeds()
    {
        var customer = Customer.Create(
            ValidFirstName,
            ValidLastName,
            ValidEmail,
            phone: null,
            passwordHash: ValidHash,
            externalId: null);

        customer.PasswordHash.Should().NotBeNull();
        customer.PasswordHash!.Value.Should().MatchRegex(@"^\$2[aby]\$");

        BCrypt.Net.BCrypt.Verify(
            PasswordFixtures.Plain,
            customer.PasswordHash.Value
        ).Should().BeTrue();
    }

    // ───────────────────────────────────────────────────────────────
    // 2. EXTERNAL IDENTITY — External provider only
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_ExternalIdentity_Succeeds()
    {
        var externalId = ExternalAuthProviderId.From("auth0|abc123");

        var customer = Customer.Create(
            ValidFirstName,
            ValidLastName,
            ValidEmail,
            ValidPhone,
            passwordHash: null,
            externalId: externalId);

        customer.Id.Value.Should().NotBe(Guid.Empty);
        customer.ExternalAuthProviderId.Should().Be(externalId);
        customer.PasswordHash.Should().BeNull();
    }

    // ───────────────────────────────────────────────────────────────
    // 3. INVALID — Both password AND external provider
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithBothPasswordAndExternalProvider_Throws()
    {
        var externalId = ExternalAuthProviderId.From("auth0|abc123");

        Action act = () => Customer.Create(
            ValidFirstName,
            ValidLastName,
            ValidEmail,
            ValidPhone,
            passwordHash: ValidHash,
            externalId: externalId);

        act.Should().Throw<ConflictingIdentitySourcesException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 4. INVALID — Neither password NOR external provider
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithNoIdentitySource_Throws()
    {
        Action act = () => Customer.Create(
            ValidFirstName,
            ValidLastName,
            ValidEmail,
            ValidPhone,
            passwordHash: null,
            externalId: null);

        act.Should().Throw<MissingIdentitySourceException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 5. INVALID — Null first name
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithNullFirstName_Throws()
    {
        Action act = () => Customer.Create(
            null!,
            ValidLastName,
            ValidEmail,
            ValidPhone,
            passwordHash: ValidHash,
            externalId: null);

        act.Should().Throw<ArgumentNullException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 6. INVALID — Empty first name
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithEmptyFirstName_Throws()
    {
        Action act = () => Customer.Create(
            FirstName.From(""),
            ValidLastName,
            ValidEmail,
            ValidPhone,
            passwordHash: ValidHash,
            externalId: null);

        act.Should().Throw<InvalidFirstNameException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 7. INVALID — Null last name
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithNullLastName_Throws()
    {
        Action act = () => Customer.Create(
            ValidFirstName,
            null!,
            ValidEmail,
            ValidPhone,
            passwordHash: ValidHash,
            externalId: null);

        act.Should().Throw<ArgumentNullException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 8. INVALID — Empty last name
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithEmptyLastName_Throws()
    {
        Action act = () => Customer.Create(
            firstName: ValidFirstName,
            lastName: LastName.From(""),
            email: ValidEmail,
            phone: ValidPhone,
            passwordHash: ValidHash,
            externalId: null);

        act.Should().Throw<InvalidLastNameException>();
    }
}
