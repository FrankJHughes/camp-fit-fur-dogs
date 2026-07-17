using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.TestUtilities.ValidServices;

public sealed class FakeCommand : ICommand<FakeResponse>
{
}
