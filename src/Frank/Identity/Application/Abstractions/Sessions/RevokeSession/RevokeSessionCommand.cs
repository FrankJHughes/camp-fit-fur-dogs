using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.Identity.Application.Abstractions.Sessions.RevokeSession;

public sealed record RevokeSessionCommand(
    string TokenHash) : ICommand;
