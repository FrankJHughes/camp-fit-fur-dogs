using Frank.Identity.Domain.Users;
using Frank.Identity.Domain.Users.Exceptions;
using Frank.TestUtilities.Fixtures;

namespace Frank.Identity.Domain.Tests.Users;

public class UserTests
{
    private static FirstName ValidFirstName => FirstName.From(NameFixtures.DefaultFirst);
    private static LastName ValidLastName => LastName.From(NameFixtures.DefaultLast);
    private static Email ValidEmail => Email.From(EmailFixtures.Default);
    private static PhoneNumber ValidPhone => PhoneNumber.From(PhoneNumberFixtures.Valid);
    private static ExternalId ValidExternalId => ExternalId.From("auth0|abc123");

    // ───────────────────────────────────────────────────────────────
    // 1. EXTERNAL IDENTITY — Required and successful creation
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_ExternalIdentity_Succeeds()
    {
        var user = User.Create(
            firstName: ValidFirstName,
            lastName: ValidLastName,
            email: ValidEmail,
            externalId: ValidExternalId,
            phone: ValidPhone);

        user.Id.Value.Should().NotBe(Guid.Empty);
        user.ExternalId.Should().Be(ValidExternalId);
        user.FirstName.Should().Be(ValidFirstName);
        user.LastName.Should().Be(ValidLastName);
        user.Email.Should().Be(ValidEmail);
        user.Phone.Should().Be(ValidPhone);
    }

    // ───────────────────────────────────────────────────────────────
    // 2. INVALID — Null ExternalId
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithNullExternalId_Throws()
    {
        Action act = () => User.Create(
            firstName: ValidFirstName,
            lastName: ValidLastName,
            email: ValidEmail,
            externalId: null!,
            phone: ValidPhone);

        act.Should().Throw<ArgumentNullException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 3. INVALID — Null first name
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithNullFirstName_Throws()
    {
        Action act = () => User.Create(
            firstName: null!,
            lastName: ValidLastName,
            email: ValidEmail,
            externalId: ValidExternalId,
            phone: ValidPhone);

        act.Should().Throw<ArgumentNullException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 4. INVALID — Empty first name
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithEmptyFirstName_Throws()
    {
        Action act = () => User.Create(
            firstName: FirstName.From(""),
            lastName: ValidLastName,
            email: ValidEmail,
            externalId: ValidExternalId,
            phone: ValidPhone);

        act.Should().Throw<InvalidFirstNameException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 5. INVALID — Null last name
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithNullLastName_Throws()
    {
        Action act = () => User.Create(
            firstName: ValidFirstName,
            lastName: null!,
            email: ValidEmail,
            externalId: ValidExternalId,
            phone: ValidPhone);

        act.Should().Throw<ArgumentNullException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 6. INVALID — Empty last name
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithEmptyLastName_Throws()
    {
        Action act = () => User.Create(
            firstName: ValidFirstName,
            lastName: LastName.From(""),
            email: ValidEmail,
            externalId: ValidExternalId,
            phone: ValidPhone);

        act.Should().Throw<InvalidLastNameException>();
    }

    // ───────────────────────────────────────────────────────────────
    // 7. INVALID — Null email
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithNullEmail_Throws()
    {
        Action act = () => User.Create(
            firstName: ValidFirstName,
            lastName: ValidLastName,
            email: null!,
            externalId: ValidExternalId,
            phone: ValidPhone);

        act.Should().Throw<ArgumentNullException>();
    }
}
