using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public static class CustomerFixtures
{
    public static FirstName First => FirstName.From(NameFixtures.DefaultFirst);
    public static LastName Last => LastName.From(NameFixtures.DefaultLast);
    public static Email Email => Email.From(EmailFixtures.Default);
    public static PhoneNumber Phone => PhoneNumber.From(PhoneNumberFixtures.Valid);
    public static PasswordHash Hash => PasswordFixtures.Hash();
}
