using FluentValidation;
namespace CampFitFurDogs.Api.Validation;

public static class ValidationExtensions
{
    public static async Task<T> Validate<T>(
        this T request,
        IValidator<T> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        return request;
    }
}
