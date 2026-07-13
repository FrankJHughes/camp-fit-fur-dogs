using Frank.Core.Application.Abstractions.Command;

namespace Frank.Identity.Application.Abstractions.Sessions.RevokeSession;

public sealed record RevokeSessionCommand(
    string TokenHash) : ICommand;
