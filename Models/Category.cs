using BlogFlow.API.Exceptions;
using BlogFlow.API.Helper;

namespace BlogFlow.API.Models;

public class Category
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string DisplayName { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public ICollection<Post> Posts { get; private set; } = [];

    private Category() { }

    public Category(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new BadRequestException(
                "Category name cannot be empty or whitespace.",
                "EMPTY_CATEGORY_NAME"
            );

        DisplayName = displayName.Trim();
        Name = SlugHelper.Normalize(displayName);
    }

    public Category(Guid id, string displayName)
        : this(displayName)
    {
        Id = id;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new BadRequestException(
                "Category name cannot be empty or whitespace.",
                "EMPTY_CATEGORY_NAME"
            );

        var sanitizedName = newName.Trim();

        if (DisplayName == sanitizedName) return;

        DisplayName = sanitizedName;
        Name = SlugHelper.Normalize(newName);
    }
}