using BlogFlow.API.Exceptions.Base;

namespace BlogFlow.API.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public string EntityName { get; }
        public IEnumerable<Guid> EntityIds { get; }

        public NotFoundException(string entityName, Guid entityId)
            : this(entityName, [entityId])
        {}

        public NotFoundException(string entityName, IEnumerable<Guid> entityIds)
            : base(
                message: BuildMessage(entityName, entityIds),
                errorCode: $"{entityName.Replace(" ", "_").ToUpper()}_NOT_FOUND",
                statusCode: StatusCodes.Status404NotFound)
        {
            var ids = entityIds.ToList(); // materialize ONCE
            EntityName = entityName;
            EntityIds = ids;
        }

        private static string BuildMessage(string entityName, IEnumerable<Guid> entityIds)
        {
            var ids = entityIds.ToList();

            return ids.Count == 1
                ? $"{entityName} with ID {ids[0]} was not found"
                : $"{entityName} with IDs ({string.Join(", ", ids)}) were not found";
        }
    }
}
