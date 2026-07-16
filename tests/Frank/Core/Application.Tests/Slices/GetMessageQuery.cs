
using Frank.Core.Application.Abstractions.Query;

namespace Frank.Core.Application.Tests.Slices;

public sealed record GetMessageQuery(int Id)
    : IQuery<GetMessageResponse>;

