namespace BlogFlow.API.DTOs.Categories
{
    public class DeleteCategoryResultDTO
    {
        public Guid DeletedCategoryId { get; init; }
        public string DeletedCategoryName { get; init; } = null!;
        public int ReassignedPostCount { get; init; }
    }
}
