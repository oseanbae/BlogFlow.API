using BlogFlow.API.Exceptions;
using BlogFlow.API.Helper;
using System.Text.RegularExpressions;

namespace BlogFlow.API.Models
{
    public class Tag
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string DisplayName { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public ICollection<PostTag> PostTags { get; private set; } = [];

        private Tag() { } // EF Core

        public Tag(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new BadRequestException("Tag name cannot be empty or whitespace.", "EMPTY_TAG_NAME");

            DisplayName = displayName.Trim();
            Name = SlugHelper.Normalize(displayName);
        }

        // SEED constructor
        public Tag(Guid id, string displayName)
            : this(displayName)
        {
            Id = id;
        }
    }
}
