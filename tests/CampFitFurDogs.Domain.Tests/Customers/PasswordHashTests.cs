using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Domain.Tests.Customers;

public class PasswordHashTests
{
    [Fact]
    public void From_with_valid_hash_succeeds()
    {
        var hash = PasswordHash.From("abc123hash");
        Assert.Equal("abc123hash", hash.Value);
    }

    [Fact]
    public void From_with_empty_string_throws()
    {
        Assert.Throws<ArgumentException>(() => PasswordHash.From(""));
    }

    [Fact]
    public void From_with_whitespace_throws()
    {
        Assert.Throws<ArgumentException>(() => PasswordHash.From("   "));
    }

    [Fact]
    public void Create_with_valid_password_returns_PasswordHash()
    {
        var hash = PasswordHash.Create("ValidP@ss1");
        Assert.NotNull(hash);
        Assert.False(string.IsNullOrWhiteSpace(hash.Value));
    }

    [Fact]
    public void Verify_correct_password_returns_true()
    {
        var hash = PasswordHash.Create("ValidP@ss1");

        Assert.True(hash.Verify("ValidP@ss1"));
    }

    [Fact]
    public void Create_produces_irreversible_hash()
    {
        var hash = PasswordHash.Create("ValidP@ss1");

        Assert.NotEqual("ValidP@ss1", hash.Value);
    }

    [Fact]
    public void Create_same_password_produces_different_hashes()
    {
        var hash1 = PasswordHash.Create("ValidP@ss1");
        var hash2 = PasswordHash.Create("ValidP@ss1");

        Assert.NotEqual(hash1.Value, hash2.Value);
    }

    [Fact]
    public void Verify_wrong_password_returns_false()
    {
        var hash = PasswordHash.Create("ValidP@ss1");

        Assert.False(hash.Verify("WrongPassword"));
    }

    [Fact]
    public void Create_with_empty_password_throws()
    {
        Assert.Throws<ArgumentException>(() => PasswordHash.Create(""));
    }

    [Fact]
    public void Create_with_null_password_throws()
    {
        Assert.Throws<ArgumentException>(() => PasswordHash.Create(null!));
    }
}
