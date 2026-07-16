
using Frank.Core.Application.Abstractions.Query;
using Frank.Core.Application.Query;
using Frank.Core.Application.Tests.Slices;
using Frank.Core.Application.Tests.TestInfrastructure;
using Frank.Core.Application.Tests.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Tests.Dispatching;

public class QueryDispatcherTests : DispatcherTestBase
{
    public QueryDispatcherTests()
    {
        WithDispatcher<QueryDispatcher, IQueryDispatcher>();
    }

    [Fact]
    public async Task Dispatch_GetMessageQuery_Returns_Response()
    {
        WithQueryHandler<GetMessageQuery, GetMessageResponse, GetMessageQueryHandler>();
        WithValidator<GetMessageQuery, GetMessageQueryValidator>();
        BuildContainer();

        var dispatcher = Provider.GetRequiredService<IQueryDispatcher>();

        var response = await dispatcher.DispatchAsync<GetMessageResponse>(
            new GetMessageQuery(1),
            CancellationToken.None);

        response.Content.Should().Be("Message #1");
    }
}

