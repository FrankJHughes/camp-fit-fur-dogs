using SharedKernel.Abstractions;

namespace TestDoubles.Dispatchers;

public sealed record TestQuery(string Value) : IQuery<string>;
