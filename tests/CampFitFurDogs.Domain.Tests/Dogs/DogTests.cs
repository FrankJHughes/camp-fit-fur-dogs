using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Domain.Tests.Dogs;

public class DogTests
{
    [Fact]
    public void Create_SetsAllProperties()
    {
        var ownerId = CustomerId.New();
        var name = DogName.Create("Biscuit");
        var breed = Breed.Create("Golden Retriever");
        var dob = new DateOnly(2022, 6, 15);
        var sex = Sex.Female;

        var dog = Dog.Create(ownerId, name, breed, dob, sex);

        Assert.NotEqual(Guid.Empty, dog.Id.Value);
        Assert.Equal(ownerId, dog.OwnerId);
        Assert.Equal(name, dog.Name);
        Assert.Equal(breed, dog.Breed);
        Assert.Equal(dob, dog.DateOfBirth);
        Assert.Equal(sex, dog.Sex);
    }

    [Fact]
    public void Create_TwoDogs_HaveDistinctIds()
    {
        var ownerId = CustomerId.New();
        var name = DogName.Create("Biscuit");
        var breed = Breed.Create("Poodle");
        var dob = new DateOnly(2023, 1, 1);

        var dog1 = Dog.Create(ownerId, name, breed, dob, Sex.Male);
        var dog2 = Dog.Create(ownerId, name, breed, dob, Sex.Male);

        Assert.NotEqual(dog1.Id, dog2.Id);
    }

    [Fact]
    public void Update_SetsAllEditableProperties()
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

        Assert.Equal("Waffles", dog.Name.Value);
        Assert.Equal("Labrador", dog.Breed.Value);
        Assert.Equal(new DateOnly(2021, 3, 10), dog.DateOfBirth);
        Assert.Equal(Sex.Male, dog.Sex);
    }

    [Fact]
    public void Update_DoesNotChangeId()
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

        Assert.Equal(originalId, dog.Id);
    }

    [Fact]
    public void Update_DoesNotChangeOwnerId()
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

        Assert.Equal(ownerId, dog.OwnerId);
    }

}
