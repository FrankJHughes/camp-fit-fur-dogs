
namespace SharedKernel.Tests.Dispatching;

public class QueryDispatcherTests : DispatcherTestBase
{
    public QueryDispatcherTests()
    {
        WithDispatcher<SharedKernel.QueryDispatcher, IQueryDispatcher>();
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

