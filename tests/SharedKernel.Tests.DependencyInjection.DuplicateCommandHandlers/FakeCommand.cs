using SharedKernel.Abstractions;

namespace SharedKernel.Tests.DependencyInjection.DuplicateCommandHandlers;

public sealed record FakeCommand(string Value) : ICommand<FakeResponse>;
