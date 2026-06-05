using CampFitFurDogs.Api.Hosting;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Hosting;

public sealed class RenderPrParserTests
{
    private readonly RenderPrParser _parser = new();

    [Theory]
    [InlineData("api-pr-123", "123")]
    [InlineData("backend-pr-999", "999")]
    [InlineData("service-preview-42", "42")]
    [InlineData("web-pr-0001", "0001")]
    public void TryParse_valid_patterns_extracts_pr_number(string input, string expected)
    {
        _parser.TryParse(input, out var pr).Should().BeTrue();
        pr.Should().Be(expected);
    }

    [Theory]
    [InlineData("api")]
    [InlineData("api-123")]
    [InlineData("api-pr")]
    [InlineData("")]
    public void TryParse_invalid_patterns_returns_false(string input)
    {
        _parser.TryParse(input, out var pr).Should().BeFalse();
        pr.Should().BeNull();
    }

    [Fact]
    public void TryParse_null_returns_false()
    {
        _parser.TryParse(null, out var pr).Should().BeFalse();
        pr.Should().BeNull();
    }

    [Fact]
    public void TryParse_handles_extra_hyphens()
    {
        _parser.TryParse("api-feature-branch-pr-77", out var pr).Should().BeTrue();
        pr.Should().Be("77");
    }
}
