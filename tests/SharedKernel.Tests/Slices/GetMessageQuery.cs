
namespace SharedKernel.Tests.Slices;

public sealed record GetMessageQuery(int Id)
    : IQuery<GetMessageResponse>;

