namespace CampFitFurDogs.Application.Authentication.Pipeline;

public enum AuthCallbackStepCategory
{
    ExchangeCode,
    FetchToken,
    FetchUser,
    ResolveIdentity,
    ResolveCustomer,
    CreateSession,
    IssueCookie,
    Finalize,
    Precondition,
    ValidateUser,
    AuditLogin,
    BuildRedirect
}
