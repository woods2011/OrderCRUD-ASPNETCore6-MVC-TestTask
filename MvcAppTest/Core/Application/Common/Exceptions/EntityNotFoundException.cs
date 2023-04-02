using MvcAppTest.Core.Domain.Exceptions;

namespace MvcAppTest.Core.Application.Common.Exceptions;

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string orderName, int entityId) : base(
        $"Entity \"{orderName}\" (with id: {entityId}) was not found.") { }
}