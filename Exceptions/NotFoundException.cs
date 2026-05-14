using BlogFlow.API.Exceptions.Base;

namespace BlogFlow.API.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public string EntityName { get; }
        public Guid EntityId { get; }

        public NotFoundException(string entityName, Guid entityId)
            : base (
                message: $"{entityName} with ID {entityId} was not found",
                errorCode: $"{entityName.Replace(" ","_").ToUpper()}_NOT_FOUND",
                statusCode: StatusCodes.Status404NotFound)
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
