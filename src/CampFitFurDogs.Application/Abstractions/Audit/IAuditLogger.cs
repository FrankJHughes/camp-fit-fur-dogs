using System;
using System.Threading.Tasks;

namespace CampFitFurDogs.Application.Abstractions.Audit;

public interface IAuditLogger
{
    Task LoginSucceeded(Guid customerId, string externalId);
}
