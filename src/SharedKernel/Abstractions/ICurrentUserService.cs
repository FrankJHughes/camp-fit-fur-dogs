namespace SharedKernel.Abstractions;

public interface ICurrentUserService
{
    Guid CurrentUserId { get; }
}
