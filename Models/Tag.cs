using BlogFlow.API.Exceptions;

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
            if (string.IsNullOrWhiteSpace(displayName))
                throw new BadRequestException("Tag name cannot be empty or whitespace.", "EMPTY_TAG_NAME");

            DisplayName = displayName.Trim();
            Name = Normalize(displayName);
        }

        public static string Normalize(string name)
            => name.Trim().ToLowerInvariant();
    }
}
