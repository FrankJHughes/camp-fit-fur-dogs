namespace SharedKernel.Tests.Slices;

public sealed record SendMessageResponse(bool Success);

public sealed record GetMessageResponse(string Content);

