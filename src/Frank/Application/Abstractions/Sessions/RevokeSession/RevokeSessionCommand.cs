using Frank.Abstractions.Command;

namespace Frank.Application.Abstractions.Sessions.RevokeSession;

public sealed record RevokeSessionCommand(
    string TokenHash) : ICommand;
