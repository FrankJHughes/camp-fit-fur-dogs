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
}
