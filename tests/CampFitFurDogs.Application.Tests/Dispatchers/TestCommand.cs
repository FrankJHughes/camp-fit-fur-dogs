using SharedKernel.Abstractions;

namespace TestDoubles.Dispatchers;

public sealed record TestCommand(string Value) : ICommand<string>;
