using CampFitFurDogs.Domain.Customers;
using SharedKernel.Domain;
using FluentValidation;

namespace CampFitFurDogs.Api.Errors;

public static class ExceptionToErrorCode
{
    public static ErrorCode Map(Exception ex) =>
        ex switch
        {
            EmailAlreadyExistsException => ErrorCode.DuplicateEmail,
            ValidationException => ErrorCode.ValidationFailed,
            DomainException => ErrorCode.DomainError,
            _ => ErrorCode.Unexpected
        };
}
