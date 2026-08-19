using CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;
using FluentValidation.TestHelper;

namespace CampFitFurDogs.Api.Tests.Abstractions.Endpoints.Dogs;

public class EditDogEndpointRequestValidatorTests
{
    private readonly EditDogEndpointRequestValidator _validator = new();

    [Fact]
    public void Valid_Request_Passes()
    {
        var req = new EditDogEndpointRequest("Biscuit", "Golden Retriever", "2022-06-15", "Female");
        var result = _validator.TestValidate(req);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Invalid_Sex_Fails()
    {
        var req = new EditDogEndpointRequest("Biscuit", "Golden Retriever", "2022-06-15", "Unknown");
        var result = _validator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.Sex);
    }
}
