namespace BlogFlow.API.Models
{
    public class Tag
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string DisplayName { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public ICollection<PostTag> PostTags { get; private set; } = [];

        public Tag() { }

        public Tag(string displayName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
            DisplayName = displayName.Trim();
            Name = Normalize(displayName);
        }

        public static string Normalize(string name)
            => name.Trim().ToLowerInvariant();
    }
}
