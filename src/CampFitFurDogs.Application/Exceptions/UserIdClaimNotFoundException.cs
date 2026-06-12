namespace CampFitFurDogs.Application.Exceptions;

public sealed class UserIdClaimNotFoundException : Exception
{
    public UserIdClaimNotFoundException()
        : base("User ID claim not found.") { }
}
