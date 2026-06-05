using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Customers.Exceptions;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class PasswordHashTests
{
    // A known-valid BCrypt hash for testing
    private const string ValidBcryptHash =
        "$2a$11$C6UzMDM.H6dfI/f/IKcEeO5Y1oCbcnZ6FQcmGeX01d9K/Y3W2FPe";

    [Fact]
    public void From_with_valid_bcrypt_hash_succeeds()
    {
        var hash = PasswordHash.From(ValidBcryptHash);
        hash.Value.Should().Be(ValidBcryptHash);
    }

    [Fact]
    public void From_with_empty_string_throws()
    {
        Action act = () => PasswordHash.From("");
        act.Should().Throw<InvalidPasswordHashException>()
           .WithMessage("*empty*");
    }

    [Fact]
    public void From_with_whitespace_throws()
    {
        Action act = () => PasswordHash.From("   ");
        act.Should().Throw<InvalidPasswordHashException>()
           .WithMessage("*empty*");
    }

    [Fact]
    public void From_with_non_bcrypt_string_throws()
    {
        Action act = () => PasswordHash.From("abc123hash");
        act.Should().Throw<InvalidPasswordHashException>()
           .WithMessage("*bcrypt*");
    }

    [Fact]
    public void Create_with_valid_password_returns_PasswordHash()
    {
        var hash = PasswordHash.Create("ValidP@ss1");

        hash.Should().NotBeNull();
        hash.Value.Should().NotBeNullOrWhiteSpace();
        hash.Value.Should().MatchRegex(@"^\$2[aby]\$");
    }

    [Fact]
    public void Verify_correct_password_returns_true()
    {
        var hash = PasswordHash.Create("ValidP@ss1");
        hash.Verify("ValidP@ss1").Should().BeTrue();
    }

    [Fact]
    public void Create_produces_irreversible_hash()
    {
        var hash = PasswordHash.Create("ValidP@ss1");
        hash.Value.Should().NotBe("ValidP@ss1");
    }

    [Fact]
    public void Create_same_password_produces_different_hashes()
    {
        var hash1 = PasswordHash.Create("ValidP@ss1");
        var hash2 = PasswordHash.Create("ValidP@ss1");

        hash1.Value.Should().NotBe(hash2.Value);
    }

    [Fact]
    public void Verify_wrong_password_returns_false()
    {
        var hash = PasswordHash.Create("ValidP@ss1");
        hash.Verify("WrongPassword").Should().BeFalse();
    }

    [Fact]
    public void Create_with_empty_password_throws()
    {
        Action act = () => PasswordHash.Create("");
        act.Should().Throw<InvalidPasswordHashException>()
           .WithMessage("*empty*");
    }

    [Fact]
    public void Create_with_null_password_throws()
    {
        Action act = () => PasswordHash.Create(null!);

        act.Should().Throw<InvalidPasswordHashException>()
           .WithMessage("*empty*");
    }
}
