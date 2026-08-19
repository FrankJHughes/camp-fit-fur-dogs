using Frank.Core.Infrastructure.Observations;

namespace Frank.Core.Infrastructure.Tests.Observations.Unit;

public class CorrelationContextTests
{
    [Fact]
    public void Create_Returns_32_Character_Id()
    {
        var ctx = new CorrelationContext();

        var id = ctx.Create();

        Assert.Equal(32, id.Length);
        Assert.True(id.All(char.IsLetterOrDigit));
    }

    [Fact]
    public void Propagate_Returns_Incoming_When_Valid()
    {
        var ctx = new CorrelationContext();

        var result = ctx.Propagate("abc123");

        Assert.Equal("abc123", result);
    }

    [Fact]
    public void Propagate_Generates_New_When_Incoming_Is_Null_Or_Whitespace()
    {
        var ctx = new CorrelationContext();

        var result1 = ctx.Propagate(null);
        var result2 = ctx.Propagate("   ");

        Assert.Equal(32, result1.Length);
        Assert.Equal(32, result2.Length);
    }
}
