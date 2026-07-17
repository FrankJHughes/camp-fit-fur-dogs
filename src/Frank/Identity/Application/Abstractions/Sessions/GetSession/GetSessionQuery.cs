using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace Frank.Identity.Application.Abstractions.Sessions.GetSession;

public record GetSessionQuery(string TokenHash) : IQuery<GetSessionResponse?>;
