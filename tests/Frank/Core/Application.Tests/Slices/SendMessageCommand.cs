
using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.Core.Application.Tests.Slices;

public sealed record SendMessageCommand(string Text)
    : ICommand<SendMessageResponse>;

