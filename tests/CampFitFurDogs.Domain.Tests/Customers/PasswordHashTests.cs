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
}
