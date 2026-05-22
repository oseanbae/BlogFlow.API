namespace BlogFlow.API.DTOs.Tag
{
    public class TagReadDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }
}
