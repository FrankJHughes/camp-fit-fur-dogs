namespace CampFitFurDogs.Application.Exceptions;

public sealed class UserNotAuthenticatedException : Exception
{
    public UserNotAuthenticatedException()
        : base("User not authenticated.") { }
}
