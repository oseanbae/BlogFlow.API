using BlogFlow.API.Models;

public class Category
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string DisplayName { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public ICollection<Post> Posts { get; private set; } = [];

    private Category() { }

    public Category(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Category name cannot be empty or whitespace.", nameof(displayName));

        DisplayName = displayName.Trim();
        Name = Normalize(displayName);
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Category name cannot be empty or whitespace.", nameof(newName));

        if (DisplayName == newName.Trim()) return;

        DisplayName = newName.Trim();
        Name = Normalize(newName);
    }

    public static string Normalize(string name)
        => name.Trim().ToLowerInvariant();
}