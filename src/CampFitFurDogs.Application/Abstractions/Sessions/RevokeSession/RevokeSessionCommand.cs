using Frank.Abstractions.Command;

namespace CampFitFurDogs.Application.Abstractions.Sessions.RevokeSession;

public sealed record RevokeSessionCommand(
    string TokenHash) : ICommand;
