using FluentAssertions;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Domain.Tests.Dogs;

public class DogTests
{
    [Fact]
    public void Create_sets_all_properties()
    {
        var ownerId = CustomerId.New();
        var name = DogName.Create("Biscuit");
        var breed = Breed.Create("Golden Retriever");
        var dob = new DateOnly(2022, 6, 15);
        var sex = Sex.Female;

        var dog = Dog.Create(ownerId, name, breed, dob, sex);

        dog.Id.Value.Should().NotBe(Guid.Empty);
        dog.OwnerId.Should().Be(ownerId);
        dog.Name.Should().Be(name);
        dog.Breed.Should().Be(breed);
        dog.DateOfBirth.Should().Be(dob);
        dog.Sex.Should().Be(sex);
    }

    [Fact]
    public void Create_two_dogs_have_distinct_ids()
    {
        var ownerId = CustomerId.New();
        var name = DogName.Create("Biscuit");
        var breed = Breed.Create("Poodle");
        var dob = new DateOnly(2023, 1, 1);

        var dog1 = Dog.Create(ownerId, name, breed, dob, Sex.Male);
        var dog2 = Dog.Create(ownerId, name, breed, dob, Sex.Male);

        dog1.Id.Should().NotBe(dog2.Id);
    }

    [Fact]
    public void Update_sets_all_editable_properties()
    {
        var ownerId = CustomerId.New();
        var dog = Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        dog.Update(
            DogName.Create("Waffles"),
            Breed.Create("Labrador"),
            new DateOnly(2021, 3, 10),
            Sex.Male);

        dog.Name.Value.Should().Be("Waffles");
        dog.Breed.Value.Should().Be("Labrador");
        dog.DateOfBirth.Should().Be(new DateOnly(2021, 3, 10));
        dog.Sex.Should().Be(Sex.Male);
    }

    [Fact]
    public void Update_does_not_change_id()
    {
        var dog = Dog.Create(
            CustomerId.New(),
            DogName.Create("Biscuit"),
            Breed.Create("Poodle"),
            new DateOnly(2023, 1, 1),
            Sex.Male);

        var originalId = dog.Id;

        dog.Update(
            DogName.Create("Waffles"),
            Breed.Create("Labrador"),
            new DateOnly(2021, 3, 10),
            Sex.Female);

        dog.Id.Should().Be(originalId);
    }

    [Fact]
    public void Update_does_not_change_owner_id()
    {
        var ownerId = CustomerId.New();
        var dog = Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Poodle"),
            new DateOnly(2023, 1, 1),
            Sex.Female);

        dog.Update(
            DogName.Create("Waffles"),
            Breed.Create("Labrador"),
            new DateOnly(2021, 3, 10),
            Sex.Male);

        dog.OwnerId.Should().Be(ownerId);
    }
}
