
using Frank.Core.Application.Abstractions.Query;

namespace Frank.Tests.Slices;

public sealed record GetMessageQuery(int Id)
    : IQuery<GetMessageResponse>;

