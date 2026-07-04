using Frank.Abstractions.Query;

namespace CampFitFurDogs.Application.Abstractions.Sessions.GetSession;

public record GetSessionQuery(string TokenHash) : IQuery<GetSessionResponse?>;
