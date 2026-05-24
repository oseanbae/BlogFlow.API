namespace BlogFlow.API.DTOs.Tag
{
    public class TagReadDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string DisplayName { get; init; } = null!;
    }
}
