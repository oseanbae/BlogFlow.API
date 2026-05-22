using BlogFlow.API.Exceptions.Base;

namespace BlogFlow.API.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public string EntityName { get; }
        public IEnumerable<Guid> EntityIds { get; }

        public NotFoundException(string entityName, Guid entityId)
            : this(entityName, [entityId]) { } 

        public NotFoundException(string entityName, IEnumerable<Guid> entityIds)
            : base(
                message: entityIds.Count() == 1
                ? $"{entityName} with ID {entityIds.First()} was not found"
                : $"{entityName} with IDs ({string.Join(", ", entityIds)}) were not found",
            errorCode: $"{entityName.Replace(" ", "_").ToUpper()}_NOT_FOUND",
            statusCode: StatusCodes.Status404NotFound)
        {
            EntityName = entityName;
            EntityIds = entityIds;
        }
    }
}
