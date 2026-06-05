using CampFitFurDogs.Api.Hosting;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace CampFitFurDogs.Api.Tests.Hosting;

public sealed class RenderConfigurationWriterTests
{
    [Fact]
    public void Apply_sets_expected_configuration_keys()
    {
        var writer = new RenderConfigurationWriter();
        var builder = WebApplication.CreateBuilder();

        writer.Apply(builder, "Server=Test;", "https://frontend");

        builder.Configuration["ConnectionStrings:DefaultConnection"]
            .Should().Be("Server=Test;");

        builder.Configuration["Frontend__BaseUrl"]
            .Should().Be("https://frontend");
    }
}
