namespace BlogFlow.API.DTOs.Categories
{
    public class CategoryReadDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string DisplayName { get; init; } = null!;
    }
}
