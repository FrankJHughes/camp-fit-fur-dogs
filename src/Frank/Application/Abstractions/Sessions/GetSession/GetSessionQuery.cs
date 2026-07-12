using Frank.Abstractions.Query;

namespace Frank.Application.Abstractions.Sessions.GetSession;

public record GetSessionQuery(string TokenHash) : IQuery<GetSessionResponse?>;
