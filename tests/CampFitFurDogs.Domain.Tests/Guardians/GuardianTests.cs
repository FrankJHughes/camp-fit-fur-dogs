using Xunit;
using CampFitFurDogs.Domain.Guardians;

namespace CampFitFurDogs.Domain.Tests.Guardians;

public class GuardianTests
{
    [Fact]
    public void Create_returns_guardian_with_correct_properties()
    {
        var guardian = Guardian.Create("Jane", "Doe", "jane@example.com", "555-0100");

        Assert.Equal("Jane", guardian.FirstName);
        Assert.Equal("Doe", guardian.LastName);
        Assert.Equal("jane@example.com", guardian.Email);
        Assert.Equal("555-0100", guardian.Phone);
    }

    [Fact]
    public void Create_assigns_new_guardian_id()
    {
        var guardian = Guardian.Create("Jane", "Doe", "jane@example.com", "555-0100");
        Assert.NotEqual(Guid.Empty, guardian.Id.Value);
    }

    [Fact]
    public void Create_with_null_first_name_throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Guardian.Create(null!, "Doe", "jane@example.com", "555-0100"));
    }

    [Fact]
    public void Create_with_empty_first_name_throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Guardian.Create("", "Doe", "jane@example.com", "555-0100"));
    }

    [Fact]
    public void Create_with_null_last_name_throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Guardian.Create("Jane", null!, "jane@example.com", "555-0100"));
    }

    [Fact]
    public void Create_with_empty_last_name_throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Guardian.Create("Jane", "", "jane@example.com", "555-0100"));
    }

    [Fact]
    public void Create_with_null_email_throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Guardian.Create("Jane", "Doe", null!, "555-0100"));
    }

    [Fact]
    public void Create_with_empty_email_throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Guardian.Create("Jane", "Doe", "", "555-0100"));
    }

    [Fact]
    public void Create_with_null_phone_throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Guardian.Create("Jane", "Doe", "jane@example.com", null!));
    }

    [Fact]
    public void Create_with_empty_phone_throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Guardian.Create("Jane", "Doe", "jane@example.com", ""));
    }
}
