namespace CampFitFurDogs.Application.Exceptions;

public sealed class UserIdClaimNotFoundException : System.Exception
{
    public UserIdClaimNotFoundException()
        : base("User ID claim not found.") { }
}
