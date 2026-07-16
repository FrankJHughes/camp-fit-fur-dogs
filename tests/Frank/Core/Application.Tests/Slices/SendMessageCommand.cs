
using Frank.Core.Application.Abstractions.Command;

namespace Frank.Core.Application.Tests.Slices;

public sealed record SendMessageCommand(string Text)
    : ICommand<SendMessageResponse>;

