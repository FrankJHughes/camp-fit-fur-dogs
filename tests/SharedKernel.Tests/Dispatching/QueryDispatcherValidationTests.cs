using SharedKernel.Tests.Fakes;

namespace SharedKernel.Tests.Dispatching;

public class QueryDispatcherValidationTests : DispatcherTestBase
{
    public QueryDispatcherValidationTests()
    {
        WithDispatcher<SharedKernel.QueryDispatcher, IQueryDispatcher>();
    }

    [Fact]
    public async Task Dispatch_InvalidQuery_Fails_Validation_And_Does_Not_Invoke_Handler()
    {
        var handler = new TrackingQueryHandler<GetMessageQuery, GetMessageResponse>();

        WithInstance<IQueryHandler<GetMessageQuery, GetMessageResponse>>(handler);
        WithValidator<GetMessageQuery, FailingValidator<GetMessageQuery>>();
        BuildContainer();

        var dispatcher = Provider.GetRequiredService<IQueryDispatcher>();

        await Assert.ThrowsAsync<ValidationException>(() =>
            dispatcher.DispatchAsync<GetMessageResponse>(
                new GetMessageQuery(0),
                CancellationToken.None));

        handler.CallCount.Should().Be(0);
    }
}

