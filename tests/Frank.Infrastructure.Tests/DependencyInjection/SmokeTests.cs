using Xunit;
using FluentAssertions;

namespace Frank.Infrastructure.Tests.DependencyInjection;

public class SmokeTests
{
    [Fact]
    public void Project_loads()
    {
        true.Should().BeTrue();
    }
}
