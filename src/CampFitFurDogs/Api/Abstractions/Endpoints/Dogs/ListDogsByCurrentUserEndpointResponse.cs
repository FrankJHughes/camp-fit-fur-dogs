namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

/// <summary>
/// Represents the response returned when listing all dogs associated with the
/// currently authenticated user.
/// <para>
/// This model provides a collection of lightweight dog summaries, suitable for
/// dashboards, list views, or any scenario where full dog details are not required.
/// </para>
/// </summary>
/// <param name="Dogs">
/// A read‑only collection of dog summaries belonging to the current user.
/// Each item contains the dog's identifier, name, and breed.
/// </param>
public record ListDogsByCurrentUserEndpointResponse(
    IReadOnlyList<GetDogSummaryEndpointResponse> Dogs);
