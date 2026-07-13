using Frank.Core.Application.Abstractions.Query;

namespace Frank.Identity.Application.Abstractions.Sessions.GetSession;

public record GetSessionQuery(string TokenHash) : IQuery<GetSessionResponse?>;
