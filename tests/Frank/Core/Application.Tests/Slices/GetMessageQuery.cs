
using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace Frank.Core.Application.Tests.Slices;

public sealed record GetMessageQuery(int Id)
    : IQuery<GetMessageResponse>;

