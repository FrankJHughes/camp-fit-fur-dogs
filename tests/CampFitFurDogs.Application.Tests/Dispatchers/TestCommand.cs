using CampFitFurDogs.Application.Abstractions;

namespace TestDoubles.Dispatchers;

public sealed record TestCommand(string Value) : ICommand<string>;
