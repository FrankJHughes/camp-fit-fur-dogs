using Frank.Core.Application.Abstractions.UnitOfWork;

namespace CampFitFurDogs.Application.Abstractions.UnitOfWork;

/// <summary>
/// Represents the application‑level unit of work for Camp Fit Fur Dogs.
/// <para>
/// This interface extends the shared <see cref="IUnitOfWork"/> abstraction and
/// provides a clear, intention‑revealing contract for coordinating transactional
/// operations within the application layer.
/// </para>
/// <para>
/// Handlers in vertical slices (such as <c>RegisterDog</c>, <c>EditDog</c>,
/// and <c>RemoveDog</c>) depend on this interface to commit or roll back
/// persistence changes atomically. Infrastructure implementations supply the
/// actual transaction behavior.
/// </para>
/// </summary>
public interface IAppUnitOfWork : IUnitOfWork { }
