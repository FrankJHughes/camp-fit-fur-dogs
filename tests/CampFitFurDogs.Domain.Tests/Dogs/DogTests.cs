using Xunit;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Domain.Guardians;

namespace CampFitFurDogs.Domain.Tests.Dogs;

public class DogTests
{
    [Fact]
    public void Create_returns_dog_with_correct_properties()
    {
        var guardianId = GuardianId.New();
        var dob = new DateOnly(2020, 6, 15);

        var dog = Dog.Create("Rex", "Labrador", dob, guardianId);

        Assert.Equal("Rex", dog.Name);
        Assert.Equal("Labrador", dog.Breed);
        Assert.Equal(dob, dog.DateOfBirth);
        Assert.Equal(guardianId, dog.GuardianId);
    }

    [Fact]
    public void Create_assigns_new_dog_id()
    {
        var dog = Dog.Create("Rex", "Labrador", new DateOnly(2020, 6, 15), GuardianId.New());
        Assert.NotEqual(Guid.Empty, dog.Id.Value);
    }

    [Fact]
    public void Create_with_null_name_throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Dog.Create(null!, "Labrador", new DateOnly(2020, 6, 15), GuardianId.New()));
    }

    [Fact]
    public void Create_with_empty_name_throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Dog.Create("", "Labrador", new DateOnly(2020, 6, 15), GuardianId.New()));
    }

    [Fact]
    public void Create_with_null_breed_throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Dog.Create("Rex", null!, new DateOnly(2020, 6, 15), GuardianId.New()));
    }

    [Fact]
    public void Create_with_empty_breed_throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Dog.Create("Rex", "", new DateOnly(2020, 6, 15), GuardianId.New()));
    }

    [Fact]
    public void Create_with_null_guardian_id_throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Dog.Create("Rex", "Labrador", new DateOnly(2020, 6, 15), null!));
    }
}
