using Frank.Domain.Users;

namespace Frank.TestUtilities.Fixtures;

public static class UserFixtures
{
    public static FirstName First => FirstName.From(NameFixtures.DefaultFirst);
    public static LastName Last => LastName.From(NameFixtures.DefaultLast);
    public static Email Email => Email.From(EmailFixtures.Default);
    public static PhoneNumber Phone => PhoneNumber.From(PhoneNumberFixtures.Valid);
}
