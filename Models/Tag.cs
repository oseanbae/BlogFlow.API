namespace BlogFlow.API.Models
{
    public class Tag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public ICollection<PostTag> PostTags { get; private set; } = [];
    }
}
